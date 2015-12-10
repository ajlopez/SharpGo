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
        private static Dictionary<string, TypeInfo> types = new Dictionary<string, TypeInfo>() { { "int", TypeInfo.Int }, { "int32", TypeInfo.Int32 }, { "uint", TypeInfo.UInt }, { "rune", TypeInfo.Int32 }, { "int16", TypeInfo.Int16 }, { "int64", TypeInfo.Int64 }, { "float32", TypeInfo.Float32 }, { "float64", TypeInfo.Float64 }, { "string", TypeInfo.String }, { "bool", TypeInfo.Bool }, { "nil", TypeInfo.Nil } };

        private static string[][] binaryoperators = new string[][] 
        {
            new string[] { "||" },
            new string[] { "&&" },
            new string[] { "==", "<", ">", "<=", ">=", "!=" },
            new string[] { "+", "-" },
            new string[] { "*", "/" }
        };

        private Stack<Token> tokens = new Stack<Token>();
        private Lexer lexer;

        public Parser(string text)
        {
            this.lexer = new Lexer(text);
        }

        public IStatementNode ParseStatementNode()
        {
            var node = this.ParseSimpleStatementNode(true);

            if (node == null)
                return null;

            this.ParseEndOfStatement();

            return node;
        }

        public TypeInfo TryParseTypeInfo()
        {
            if (this.TryParseToken(TokenType.Delimiter, "["))
            {
                IExpressionNode lexpr = this.ParseExpressionNode();

                this.ParseToken(TokenType.Delimiter, "]");

                TypeInfo typeinfo = this.TryParseTypeInfo();

                if (typeinfo == null)
                    throw new ParserException("Expected type info");

                if (lexpr == null)
                    return new SliceTypeInfo(typeinfo);
                else
                    return new ArrayTypeInfo(typeinfo, lexpr);
            }

            if (this.TryParseToken(TokenType.Operator, "*"))
            {
                TypeInfo typeinfo = this.TryParseTypeInfo();

                if (typeinfo == null)
                    throw new ParserException("Expected type info");

                return new PointerTypeInfo(typeinfo);
            }

            var token = this.NextToken();

            if (token == null || token.Type != TokenType.Name || !types.ContainsKey(token.Value))
            {
                this.PushToken(token);
                return null;
            }

            return types[token.Value];
        }

        public IExpressionNode ParseExpressionNode()
        {
            return this.ParseBinaryExpressionNode(0);
        }

        private IExpressionNode ParseBinaryExpressionNode(int level)
        {
            if (level >= binaryoperators.Length)
                return this.ParseTerm();

            IExpressionNode expr = this.ParseBinaryExpressionNode(level + 1);

            if (expr == null)
                return null;

            var token = this.NextToken();

            while (token != null && token.Type == TokenType.Operator && binaryoperators[level].Contains(token.Value))
            {
                if (token.Value == "+")
                    expr = new BinaryNode(expr, BinaryOperator.Add, this.ParseBinaryExpressionNode(level + 1));
                else if (token.Value == "-")
                    expr = new BinaryNode(expr, BinaryOperator.Substract, this.ParseBinaryExpressionNode(level + 1));
                else if (token.Value == "*")
                    expr = new BinaryNode(expr, BinaryOperator.Multiply, this.ParseBinaryExpressionNode(level + 1));
                else if (token.Value == "/")
                    expr = new BinaryNode(expr, BinaryOperator.Divide, this.ParseBinaryExpressionNode(level + 1));
                else if (token.Value == "==")
                    expr = new BinaryNode(expr, BinaryOperator.Equal, this.ParseBinaryExpressionNode(level + 1));
                else if (token.Value == "!=")
                    expr = new BinaryNode(expr, BinaryOperator.NotEqual, this.ParseBinaryExpressionNode(level + 1));
                else if (token.Value == "<")
                    expr = new BinaryNode(expr, BinaryOperator.Less, this.ParseBinaryExpressionNode(level + 1));
                else if (token.Value == ">")
                    expr = new BinaryNode(expr, BinaryOperator.Greater, this.ParseBinaryExpressionNode(level + 1));
                else if (token.Value == "<=")
                    expr = new BinaryNode(expr, BinaryOperator.LessEqual, this.ParseBinaryExpressionNode(level + 1));
                else if (token.Value == ">=")
                    expr = new BinaryNode(expr, BinaryOperator.GreaterEqual, this.ParseBinaryExpressionNode(level + 1));
                else if (token.Value == "&&")
                    expr = new BinaryNode(expr, BinaryOperator.And, this.ParseBinaryExpressionNode(level + 1));
                else if (token.Value == "||")
                    expr = new BinaryNode(expr, BinaryOperator.Or, this.ParseBinaryExpressionNode(level + 1));
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
                if (this.TryParseToken(TokenType.Operator, ":="))
                    return new VarNode(token.Value, this.ParseExpressionNode());
                if (this.TryParseToken(TokenType.Delimiter, ":"))
                    return new LabelNode(token.Value, this.ParseSimpleStatementNode());
            }

            this.PushToken(token);

            if (token.Type == TokenType.Delimiter && token.Value == "{")
                return this.ParseStatementBlock();

            if (this.TryParseToken(TokenType.Name, "var"))
            {
                var name = this.ParseName();
                IExpressionNode expr = null;
                TypeInfo typeinfo = this.TryParseTypeInfo();

                if (this.TryParseToken(TokenType.Operator, "="))
                    expr = this.ParseExpressionNode();

                return new VarNode(name, typeinfo, expr);
            }

            if (this.TryParseToken(TokenType.Name, "type"))
            {
                var name = this.ParseName();
                TypeInfo typeinfo = this.TryParseTypeInfo();

                return new AliasTypeNode(name, typeinfo);
            }

            if (this.TryParseToken(TokenType.Name, "if"))
            {
                INode stmt = null;
                
                var fstmt = this.ParseSimpleStatementNode();

                if (this.TryParseToken(TokenType.Delimiter, ";"))
                {
                    stmt = fstmt;

                    fstmt = this.ParseSimpleStatementNode();
                }

                var expr = ((ExpressionStatementNode)fstmt).ExpressionNode;

                return new IfNode(stmt, expr, this.ParseStatementBlock());
            }

            if (this.TryParseToken(TokenType.Name, "for"))
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

            if (this.TryParseToken(TokenType.Name, "return"))
            {
                var expr = this.ParseExpressionNode();
                return new ReturnNode(expr);
            }

            if (this.TryParseToken(TokenType.Name, "defer"))
            {
                var expr = this.ParseExpressionNode();
                return new DeferNode(expr);
            }

            if (this.TryParseToken(TokenType.Name, "go"))
            {
                var expr = this.ParseExpressionNode();
                return new GoNode(expr);
            }

            if (this.TryParseToken(TokenType.Name, "import"))
            {
                var expr = this.ParseExpressionNode();
                return new ImportNode(expr);
            }

            if (this.TryParseToken(TokenType.Name, "break"))
            {
                string label = this.TryParseName();
                return new BreakNode(label);
            }

            if (this.TryParseToken(TokenType.Name, "continue"))
            {
                string label = this.TryParseName();
                return new ContinueNode(label);
            }

            if (this.TryParseToken(TokenType.Name, "goto"))
            {
                string label = this.ParseName();
                return new GotoNode(label);
            }

            if (this.TryParseToken(TokenType.Name, "package"))
            {
                string name = this.ParseName();
                return new PackageNode(name);
            }

            if (this.TryParseToken(TokenType.Name, "func"))
            {
                IList<NameNode> arguments = new List<NameNode>();

                var name = this.ParseName();
                this.ParseToken(TokenType.Delimiter, "(");
                this.ParseToken(TokenType.Delimiter, ")");

                this.EnsureToken(TokenType.Delimiter, "{");

                var body = this.ParseStatementNode();

                return new FuncNode(name, arguments, body);
            }

            IExpressionNode node = this.ParseExpressionNode();

            if (node == null)
                return null;

            if (this.TryParseToken(TokenType.Operator, "="))
            {
                var cmdnode = new AssignmentNode(node, this.ParseExpressionNode());
                return cmdnode;
            }

            if (this.TryParseToken(TokenType.Operator, "<-"))
            {
                var cmdnode = new SendNode(node, this.ParseExpressionNode());
                return cmdnode;
            }

            var cmd = new ExpressionStatementNode(node);
            return cmd;
        }

        private BlockNode ParseStatementBlock()
        {
            this.ParseToken(TokenType.Delimiter, "{");
            var stmts = new List<IStatementNode>();

            while (!this.TryParseToken(TokenType.Delimiter, "}"))
            {
                var stm = this.ParseStatementNode();
                stmts.Add(stm);
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

            if (token.Type == TokenType.Name)
            {
                if (this.TryParseToken(TokenType.Delimiter, "["))
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

                if (this.TryParseToken(TokenType.Delimiter, "("))
                {
                    IList<IExpressionNode> expressions = new List<IExpressionNode>();

                    for (var expr = this.ParseExpressionNode(); expr != null;)
                    {
                        expressions.Add(expr);

                        if (!this.TryParseToken(TokenType.Delimiter, ","))
                            break;

                        expr = this.ParseExpressionNode();
                    }

                    this.ParseToken(TokenType.Delimiter, ")");
                    return new CallNode(new NameNode(token.Value), expressions);
                }

                if (this.TryParseToken(TokenType.Delimiter, "."))
                {
                    string name2 = this.ParseName();
                    return new DotNode(new NameNode(token.Value), name2);
                }

                if (token.Value == "true")
                    return new ConstantNode(true);
                if (token.Value == "false")
                    return new ConstantNode(false);
                if (token.Value == "nil")
                    return new ConstantNode(null);

                return new NameNode(token.Value);
            }

            this.PushToken(token);

            return null;
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
