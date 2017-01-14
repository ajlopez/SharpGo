namespace SharpGo.Core.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Ast;
    using SharpGo.Core.Language;

    public class Parser
    {
        private static Dictionary<string, TypeInfo> types = new Dictionary<string, TypeInfo>() { { "int", TypeInfo.Int }, { "int32", TypeInfo.Int32 }, { "uint", TypeInfo.UInt }, { "rune", TypeInfo.Int32 }, { "int16", TypeInfo.Int16 }, { "int64", TypeInfo.Int64 }, { "float32", TypeInfo.Float32 }, { "float64", TypeInfo.Float64 }, { "string", TypeInfo.String }, { "bool", TypeInfo.Bool }, { "nil", TypeInfo.Nil }, { "complex64", TypeInfo.Complex64 }, { "complex128", TypeInfo.Complex128 } };
        private static Dictionary<string, BinaryOperator> bops = new Dictionary<string, BinaryOperator>() { { "+", BinaryOperator.Add }, { "-", BinaryOperator.Substract }, { "*", BinaryOperator.Multiply }, { "/", BinaryOperator.Divide }, { "%", BinaryOperator.Mod }, { "<", BinaryOperator.Less }, { "<=", BinaryOperator.LessEqual }, { ">", BinaryOperator.Greater }, { ">=", BinaryOperator.GreaterEqual }, { "<<", BinaryOperator.LeftShift }, { ">>", BinaryOperator.RightShift }, { "&", BinaryOperator.BitwiseAnd }, { "|", BinaryOperator.BitwiseOr }, { "^", BinaryOperator.BitwiseXor }, { "&^", BinaryOperator.BitwiseAndNot }, { "&&", BinaryOperator.And }, { "||", BinaryOperator.Or }, { "==", BinaryOperator.Equal }, { "!=", BinaryOperator.NotEqual } };

        private static string[][] binaryoperators = new string[][] 
        {
            new string[] { "||" },
            new string[] { "&&" },
            new string[] { "==", "<", ">", "<=", ">=", "!=" },
            new string[] { "+", "-", "|", "^" },
            new string[] { "*", "/", "%", "<<", ">>", "&", "&^" }
        };

        private Stack<Token> tokens = new Stack<Token>();
        private Lexer lexer;

        public Parser(string text)
        {
            this.lexer = new Lexer(text);
        }

        public IStatementNode ParseStatementNode()
        {
            var token = this.NextToken();

            if (token != null && token.Type == TokenType.NewLine)
                return new EmptyNode();

            this.PushToken(token);

            var node = this.ParseSimpleStatementNode(true);

            if (node == null)
                if (this.TryParseEndOfStatement())
                    return new EmptyNode();
                else
                    return null;

            if (!(node is BlockNode))
                this.ParseEndOfStatement();

            return node;
        }

        public IExpressionNode ParseExpressionNode()
        {
            return this.ParseBinaryExpressionNode(0);
        }

        private TypeInfo ParseTypeInfo()
        {
            TypeInfo typeinfo = this.TryParseTypeInfo();

            if (typeinfo == null)
                throw new ParserException("Expected type info");

            return typeinfo;
        }

        private TypeInfo TryParseTypeInfo()
        {
            if (this.TryParseToken(TokenType.Delimiter, "["))
                return ParseArraySliceTypeInfo();

            if (this.TryParseToken(TokenType.Operator, "*"))
            {
                TypeInfo typeinfo = this.ParseTypeInfo();

                return new PointerTypeInfo(typeinfo);
            }

            if (this.TryParseToken(TokenType.Name, "chan"))
                return ParseChanTypeInfo();

            if (this.TryParseToken(TokenType.Name, "map"))
                return ParseMapTypeInfo();

            if (this.TryParseToken(TokenType.Operator, "<-"))
                return ParseChannelTypeInfo();

            var token = this.NextToken();

            if (token == null || token.Type != TokenType.Name || !types.ContainsKey(token.Value))
            {
                this.PushToken(token);
                return null;
            }

            return types[token.Value];
        }

        private TypeInfo ParseArraySliceTypeInfo()
        {
            IExpressionNode lexpr = this.ParseExpressionNode();

            this.ParseToken(TokenType.Delimiter, "]");

            TypeInfo typeinfo = this.ParseTypeInfo();

            if (lexpr == null)
                return new SliceTypeInfo(typeinfo);
            else
                return new ArrayTypeInfo(typeinfo, lexpr);
        }

        private TypeInfo ParseChannelTypeInfo()
        {
            this.ParseToken(TokenType.Name, "chan");
            TypeInfo typeinfo = this.ParseTypeInfo();

            return new ChannelTypeInfo(typeinfo, null);
        }

        private TypeInfo ParseChanTypeInfo()
        {
            bool issend = this.TryParseToken(TokenType.Operator, "<-");
            TypeInfo typeinfo = this.ParseTypeInfo();

            if (issend)
                return new ChannelTypeInfo(null, typeinfo);

            return new ChannelTypeInfo(typeinfo);
        }

        private TypeInfo ParseMapTypeInfo()
        {
            this.ParseToken(TokenType.Delimiter, "[");
            TypeInfo keytypeinfo = this.ParseTypeInfo();

            this.ParseToken(TokenType.Delimiter, "]");

            TypeInfo elementtypeinfo = this.ParseTypeInfo();

            return new MapTypeInfo(keytypeinfo, elementtypeinfo);
        }

        private IExpressionNode ParseBlockExpressionNode()
        {
            if (!this.TryParseToken(TokenType.Delimiter, "{"))
                return null;

            IList<IExpressionNode> exprs = new List<IExpressionNode>();

            while (!this.TryParseToken(TokenType.Delimiter, "}"))
            {
                if (exprs.Count > 0)
                    this.ParseToken(TokenType.Delimiter, ",");

                exprs.Add(this.ParseExpressionNode());
            }

            return new ExpressionBlockNode(exprs);
        }

        private IExpressionNode ParseBinaryExpressionNode(int level)
        {
            IExpressionNode expr;

            if (level >= binaryoperators.Length)
            {
                expr = this.ParseTerm();

                while (true)
                {
                    if (this.TryParseToken(TokenType.Operator, "++"))
                    {
                        expr = new UnaryNode(expr, UnaryOperator.PostIncrement);
                        continue;
                    }

                    if (this.TryParseToken(TokenType.Operator, "--"))
                    {
                        expr = new UnaryNode(expr, UnaryOperator.PostDecrement);
                        continue;
                    }

                    break;
                }

                return expr;
            }

            expr = this.ParseBinaryExpressionNode(level + 1);

            if (expr == null)
                return null;

            var token = this.NextToken();

            while (token != null && token.Type == TokenType.Operator && binaryoperators[level].Contains(token.Value))
            {
                if (bops.ContainsKey(token.Value))
                    expr = new BinaryNode(expr, bops[token.Value], this.ParseBinaryExpressionNode(level + 1));
                else
                    break;

                token = this.NextToken();
            }

            if (token != null)
                this.PushToken(token);

            return expr;
        }

        private IStatementNode ParseSimpleStatementNode(bool canHaveLabel = false)
        {
            var token = this.NextToken();

            while (token != null && token.Type == TokenType.NewLine)
                token = this.NextToken();

            if (token == null)
                return null;

            if (token.Type == TokenType.Name) 
            {
                IStatementNode stmt = this.TryParseNameStatement(token.Value);

                if (stmt != null)
                    return stmt;
            }

            this.PushToken(token);

            if (token.Type == TokenType.Delimiter && token.Value == "{")
                return this.ParseStatementBlock();

            IExpressionNode node = this.ParseExpressionNode();

            if (node == null)
                return null;

            AssignmentNode anode = this.TryParseAssignmentNode(node);

            if (anode != null)
                return anode;

            if (this.TryParseToken(TokenType.Operator, "<-"))
            {
                var cmdnode = new SendNode(node, this.ParseExpressionNode());
                return cmdnode;
            }

            var cmd = new ExpressionStatementNode(node);

            return cmd;
        }

        private AssignmentNode TryParseAssignmentNode(IExpressionNode node)
        {
            if (this.TryParseToken(TokenType.Operator, "="))
            {
                var cmdnode = new AssignmentNode(node, this.ParseExpressionNode());
                return cmdnode;
            }

            if (this.TryParseToken(TokenType.Operator, "+="))
            {
                var cmdnode = new AssignmentNode(AssignmentOperator.Add, node, this.ParseExpressionNode());
                return cmdnode;
            }

            if (this.TryParseToken(TokenType.Operator, "-="))
            {
                var cmdnode = new AssignmentNode(AssignmentOperator.Subtract, node, this.ParseExpressionNode());
                return cmdnode;
            }

            if (this.TryParseToken(TokenType.Operator, "*="))
            {
                var cmdnode = new AssignmentNode(AssignmentOperator.Multiply, node, this.ParseExpressionNode());
                return cmdnode;
            }

            if (this.TryParseToken(TokenType.Operator, "/="))
            {
                var cmdnode = new AssignmentNode(AssignmentOperator.Divide, node, this.ParseExpressionNode());
                return cmdnode;
            }

            if (this.TryParseToken(TokenType.Operator, "%="))
            {
                var cmdnode = new AssignmentNode(AssignmentOperator.Modulus, node, this.ParseExpressionNode());
                return cmdnode;
            }

            if (this.TryParseToken(TokenType.Operator, "<<="))
            {
                var cmdnode = new AssignmentNode(AssignmentOperator.LeftShift, node, this.ParseExpressionNode());
                return cmdnode;
            }

            if (this.TryParseToken(TokenType.Operator, ">>="))
            {
                var cmdnode = new AssignmentNode(AssignmentOperator.RightShift, node, this.ParseExpressionNode());
                return cmdnode;
            }

            if (this.TryParseToken(TokenType.Operator, "|="))
            {
                var cmdnode = new AssignmentNode(AssignmentOperator.BitOr, node, this.ParseExpressionNode());
                return cmdnode;
            }

            if (this.TryParseToken(TokenType.Operator, "&="))
            {
                var cmdnode = new AssignmentNode(AssignmentOperator.BitAnd, node, this.ParseExpressionNode());
                return cmdnode;
            }

            if (this.TryParseToken(TokenType.Operator, "^="))
            {
                var cmdnode = new AssignmentNode(AssignmentOperator.BitXor, node, this.ParseExpressionNode());
                return cmdnode;
            }

            if (this.TryParseToken(TokenType.Operator, "&^="))
            {
                var cmdnode = new AssignmentNode(AssignmentOperator.BitClear, node, this.ParseExpressionNode());
                return cmdnode;
            }

            return null;
        }

        private IStatementNode TryParseNameStatement(string name)
        {
            if (name == "var")
                return this.ParseVarNode();
            if (name == "const")
                return this.ParseConstNode();
            if (name == "struct")
                return this.ParseStructNode();
            if (name == "if")
                return this.ParseIfNode();
            if (name == "for")
                return this.ParseForNode();
            if (name == "func")
                return this.ParseFuncNode();
            if (name == "type")
                return this.ParseAliasTypeNode();
            if (name == "return")
                return new ReturnNode(this.ParseExpressionNode());
            if (name == "fallthrough")
                return new FallthroughNode();
            if (name == "defer")
                return new DeferNode(this.ParseExpressionNode());
            if (name == "go")
                return new GoNode(this.ParseExpressionNode());
            if (name == "import")
                return new ImportNode(this.ParseExpressionNode());
            if (name == "break")
                return new BreakNode(this.TryParseName());
            if (name == "continue")
                return new ContinueNode(this.TryParseName());
            if (name == "goto")
                return new GotoNode(this.ParseName());
            if (name == "package")
                return new PackageNode(this.ParseName());

            if (this.TryParseToken(TokenType.Operator, ":="))
                return this.ParseVarAssignmentNode(name);

            if (this.TryParseToken(TokenType.Delimiter, ":"))
                return new LabelNode(name, this.ParseSimpleStatementNode());

            return null;
        }

        private IStatementNode ParseAliasTypeNode()
        {
            var name = this.ParseName();
            TypeInfo typeinfo = this.ParseTypeInfo();

            return new AliasTypeNode(name, typeinfo);
        }

        private IStatementNode ParseFuncNode()
        {
            IList<NameNode> arguments = new List<NameNode>();

            var name = this.ParseName();
            this.ParseToken(TokenType.Delimiter, "(");
            this.ParseToken(TokenType.Delimiter, ")");

            this.EnsureToken(TokenType.Delimiter, "{");

            var body = this.ParseStatementNode();
            TypeInfo typeInfo = this.TryParseTypeInfo();

            return new FuncNode(name, arguments, body, typeInfo);
        }

        private IStatementNode ParseForNode()
        {
            IStatementNode initstmt = this.ParseSimpleStatementNode();
            IExpressionNode expr = null;

            if (this.TryParseToken(TokenType.Delimiter, ";"))
            {
                expr = this.ParseExpressionNode();
                this.ParseToken(TokenType.Delimiter, ";");
                IStatementNode poststmt = this.ParseSimpleStatementNode();
                return new ForNode(initstmt, expr, poststmt, this.ParseStatementBlock());
            }

            expr = ((ExpressionStatementNode)initstmt).ExpressionNode;

            return new ForNode(expr, this.ParseStatementBlock());
        }

        private IStatementNode ParseIfNode()
        {
            IStatementNode stmt = null;

            var fstmt = this.ParseSimpleStatementNode();

            if (this.TryParseToken(TokenType.Delimiter, ";"))
            {
                stmt = fstmt;

                fstmt = this.ParseSimpleStatementNode();
            }

            var expr = ((ExpressionStatementNode)fstmt).ExpressionNode;

            var thenCmd = this.ParseStatementBlock();
            IStatementNode elseCmd = null;

            if (this.TryParseName("else"))
                elseCmd = this.ParseStatementBlock();

            return new IfNode(stmt, expr, thenCmd, elseCmd);
        }

        private IStatementNode ParseStructNode()
        {
            IList<StructMemberNode> members = new List<StructMemberNode>();
            this.ParseToken(TokenType.Delimiter, "{");
            this.ParseEndOfStatement();

            for (var member = this.ParseStructMemberNode(); member != null; member = this.ParseStructMemberNode())
                members.Add(member);

            this.ParseToken(TokenType.Delimiter, "}");

            return new StructNode(members);
        }

        private IStatementNode ParseConstNode()
        {
            var name = this.ParseName();
            TypeInfo typeinfo = this.TryParseTypeInfo();

            this.ParseToken(TokenType.Operator, "=");
            IExpressionNode expr = this.ParseExpressionNode();

            return new ConstNode(name, typeinfo, expr);
        }

        private IStatementNode ParseVarAssignmentNode(string name)
        {
            TypeInfo typeinfo = this.TryParseTypeInfo();

            if (typeinfo is ArrayTypeInfo || typeinfo is SliceTypeInfo)
                return new VarNode(name, typeinfo, this.ParseBlockExpressionNode());

            return new VarNode(name, typeinfo, this.ParseExpressionNode());
        }

        private VarNode ParseVarNode()
        {
            var name = this.ParseName();
            IExpressionNode expr = null;
            TypeInfo typeinfo = this.TryParseTypeInfo();

            if (this.TryParseToken(TokenType.Operator, "="))
                expr = this.ParseExpressionNode();

            return new VarNode(name, typeinfo, expr);
        }

        private StructMemberNode ParseStructMemberNode()
        {
            var tinfo = this.TryParseTypeInfo();

            if (tinfo != null) 
            {
                this.ParseEndOfStatement();
                return new StructMemberNode(null, tinfo);
            }

            var name = this.TryParseName();

            if (name == null)
                return null;

            tinfo = this.ParseTypeInfo();

            this.ParseEndOfStatement();

            return new StructMemberNode(name, tinfo);
        }

        private BlockNode ParseStatementBlock()
        {
            this.ParseToken(TokenType.Delimiter, "{");

            while (this.TryParseToken(TokenType.NewLine))
                ;

            var stmts = new List<IStatementNode>();

            while (!this.TryParseToken(TokenType.Delimiter, "}"))
            {
                var stm = this.ParseStatementNode();
                stmts.Add(stm);
                while (this.TryParseToken(TokenType.NewLine))
                    ;
            }

            return new BlockNode(stmts);
        }

        private IExpressionNode ParseTerm()
        {
            var token = this.NextToken();

            if (token == null)
                return null;

            if (token.Type == TokenType.Integer)
                return new ConstantNode(int.Parse(token.Value, CultureInfo.InvariantCulture));

            if (token.Type == TokenType.Real)
                return new ConstantNode(double.Parse(token.Value, CultureInfo.InvariantCulture));

            if (token.Type == TokenType.String)
                return new ConstantNode(token.Value);

            if (token.Type == TokenType.Operator)
            {
                UnaryNode uexpr = this.TryParseUnaryOperation(token.Value);

                if (uexpr != null)
                    return uexpr;
            }

            if (token.Type == TokenType.Delimiter && token.Value == "(")
                return ParseSubExpression();

            if (token.Type == TokenType.Name)
                return ParseNameExpression(token);

            this.PushToken(token);

            return null;
        }

        private IExpressionNode ParseNameExpression(Token token)
        {
            if (this.TryParseToken(TokenType.Delimiter, "["))
                return ParseIndexExpression(token);

            if (this.TryParseToken(TokenType.Delimiter, "("))
                return ParseCall(token);

            if (this.TryParseToken(TokenType.Delimiter, "."))
                return ParseDotName(token);

            if (token.Value == "true")
                return new ConstantNode(true);
            if (token.Value == "false")
                return new ConstantNode(false);
            if (token.Value == "nil")
                return new ConstantNode(null);

            return new NameNode(token.Value);
        }

        private UnaryNode TryParseUnaryOperation(string oper)
        {
            if (oper == "+")
                return new UnaryNode(this.ParseTerm(), UnaryOperator.Plus);
            if (oper == "-")
                return new UnaryNode(this.ParseTerm(), UnaryOperator.Minus);
            if (oper == "++")
                return new UnaryNode(this.ParseTerm(), UnaryOperator.Increment);
            if (oper == "--")
                return new UnaryNode(this.ParseTerm(), UnaryOperator.Decrement);
            if (oper == "!")
                return new UnaryNode(this.ParseTerm(), UnaryOperator.Negate);
            if (oper == "~")
                return new UnaryNode(this.ParseTerm(), UnaryOperator.BitNegate);
            if (oper == "&")
                return new UnaryNode(this.ParseTerm(), UnaryOperator.Address);
            if (oper == "*")
                return new UnaryNode(this.ParseTerm(), UnaryOperator.Pointer);

            return null;
        }

        private IExpressionNode ParseSubExpression()
        {
            var ti = this.TryParseTypeInfo();

            if (ti != null)
            {
                this.ParseToken(TokenType.Delimiter, ")");
                this.ParseToken(TokenType.Delimiter, "(");
                var expr = this.ParseExpressionNode();
                this.ParseToken(TokenType.Delimiter, ")");

                return new ConversionNode(ti, expr);
            }
            else
            {
                var expr = this.ParseExpressionNode();
                this.ParseToken(TokenType.Delimiter, ")");

                return expr;
            }
        }

        private IExpressionNode ParseDotName(Token token)
        {
            string name2 = this.ParseName();
            return new DotNode(new NameNode(token.Value), name2);
        }

        private IExpressionNode ParseIndexExpression(Token token)
        {
            IExpressionNode expression = this.ParseExpressionNode();

            if (this.TryParseToken(TokenType.Delimiter, ":"))
            {
                IExpressionNode expression2 = this.ParseExpressionNode();
                this.ParseToken(TokenType.Delimiter, "]");
                return new SliceNode(token.Value, expression, expression2);
            }

            this.ParseToken(TokenType.Delimiter, "]");

            return new IndexedNode(token.Value, expression);
        }

        private IExpressionNode ParseCall(Token token)
        {
            IList<IExpressionNode> expressions = new List<IExpressionNode>();

            for (var expr = this.ParseExpressionNode(); expr != null; )
            {
                expressions.Add(expr);

                if (!this.TryParseToken(TokenType.Delimiter, ","))
                    break;

                expr = this.ParseExpressionNode();
            }

            this.ParseToken(TokenType.Delimiter, ")");

            if (types.ContainsKey(token.Value) && expressions.Count == 1)
                return new ConversionNode(types[token.Value], expressions[0]);

            return new CallNode(new NameNode(token.Value), expressions);
        }

        private string ParseName()
        {
            var token = this.NextToken();

            if (token == null || token.Type != TokenType.Name)
                throw new ParserException("Expected a name");

            return token.Value;
        }

        private Token NextToken()
        {
            if (this.tokens.Count > 0)
                return this.tokens.Pop();

            return this.lexer.NextToken();
        }

        private void PushToken(Token token)
        {
            this.tokens.Push(token);
        }

        private void ParseToken(TokenType type, string value)
        {
            var token = this.NextToken();

            if (token == null || !(token.Type == type && token.Value == value))
                throw new ParserException(string.Format("Expected '{0}'", value));
        }

        private void EnsureToken(TokenType type, string value)
        {
            var token = this.NextToken();

            if (token == null || !(token.Type == type && token.Value == value))
                throw new ParserException(string.Format("Expected '{0}'", value));

            this.PushToken(token);
        }

        private bool TryParseToken(TokenType type)
        {
            var token = this.NextToken();

            if (token == null)
                return false;

            if (token.Type == type)
                return true;

            this.PushToken(token);

            return false;
        }

        private bool TryParseToken(TokenType type, string value)
        {
            var token = this.NextToken();

            if (token == null)
                return false;

            if (token.Type == type && token.Value == value)
                return true;

            this.PushToken(token);
            return false;
        }

        private string TryParseName()
        {
            var token = this.NextToken();

            if (token == null)
                return null;

            if (token.Type == TokenType.Name)
                return token.Value;

            this.PushToken(token);

            return null;
        }

        private bool TryParseName(string name)
        {
            var token = this.NextToken();

            if (token == null)
                return false;

            if (token.Type == TokenType.Name && token.Value == name)
                return true;

            this.PushToken(token);

            return false;
        }

        private bool TryParseEndOfStatement()
        {
            var token = this.NextToken();

            if (token == null)
                return false;

            if (token.Type == TokenType.NewLine)
                return true;

            if (token.Type == TokenType.Delimiter && token.Value == ";")
                return true;

            if (token.Type == TokenType.Delimiter && token.Value == "}")
            {
                this.PushToken(token);
                return true;
            }

            return false;
        }
        private void ParseEndOfStatement()
        {
            var token = this.NextToken();

            if (token == null)
                return;

            if (token.Type == TokenType.NewLine)
                return;

            if (token.Type == TokenType.Delimiter && token.Value == ";")
                return;

            if (token.Type == TokenType.Delimiter && token.Value == "}")
            {
                this.PushToken(token);
                return;
            }

            throw new ParserException(string.Format("Unexpected '{0}'", token.Value));
        }
    }
}
