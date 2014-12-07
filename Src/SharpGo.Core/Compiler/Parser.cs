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
        private Stack<Token> tokens = new Stack<Token>();
        private Lexer lexer;
        private string[][] binaryoperators = new string[][] 
        {
            new string[] { "||" },
            new string[] { "&&" },
            new string[] { "==", "<", ">", "<=", ">=", "!=" },
            new string[] { "+", "-" },
            new string[] { "*", "/" }
        };

        public Parser(string text)
        {
            this.lexer = new Lexer(text);
        }

        public INode ParseStatementNode()
        {
            var node = this.ParseSimpleStatementNode();

            if (node == null)
                return null;

            this.ParseEndOfStatement();

            return node;
        }

        public TypeInfo TryParseTypeInfo()
        {
            if (this.TryParseName("int32"))
                return TypeInfo.Int32;

            return null;
        }

        public IExpressionNode ParseExpressionNode()
        {
            return this.ParseBinaryExpressionNode(0);
        }

        private IExpressionNode ParseBinaryExpressionNode(int level)
        {
            if (level >= this.binaryoperators.Length)
                return this.ParseTerm();

            IExpressionNode expr = this.ParseBinaryExpressionNode(level + 1);

            if (expr == null)
                return null;

            var token = this.NextToken();

            while (token != null && token.Type == TokenType.Operator && this.binaryoperators[level].Contains(token.Value))
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

        private INode ParseSimpleStatementNode()
        {
            var token = this.NextToken();

            while (token != null && token.Type == TokenType.NewLine)
                token = this.NextToken();

            if (token == null)
                return null;

            this.PushToken(token);

            if (this.TryParseToken(TokenType.Delimiter, "{"))
            {
                var stmts = new List<INode>();

                while (!this.TryParseToken(TokenType.Delimiter, "}"))
                {
                    var stmt = this.ParseStatementNode();
                    stmts.Add(stmt);
                }

                return new BlockNode(stmts);
            }

            if (this.TryParseToken(TokenType.Name, "var"))
            {
                var name = this.ParseName();
                IExpressionNode expr = null;
                TypeInfo typeinfo = this.TryParseTypeInfo();

                if (this.TryParseToken(TokenType.Operator, "="))
                    expr = this.ParseExpressionNode();

                return new VarNode(name, typeinfo, expr);
            }

            if (this.TryParseToken(TokenType.Name, "if"))
            {
                var fstmt = this.ParseSimpleStatementNode();
                var expr = ((ExpressionStatementNode)fstmt).ExpressionNode;
                this.ParseToken(TokenType.Delimiter, "{");
                var stmts = new List<INode>();

                while (!this.TryParseToken(TokenType.Delimiter, "}"))
                {
                    var stmt = this.ParseStatementNode();
                    stmts.Add(stmt);
                }

                return new IfNode(expr, new BlockNode(stmts));
            }

            if (this.TryParseToken(TokenType.Name, "for"))
            {
                var expr = this.ParseExpressionNode();
                this.ParseToken(TokenType.Delimiter, "{");
                var stmts = new List<INode>();

                while (!this.TryParseToken(TokenType.Delimiter, "}"))
                {
                    var stmt = this.ParseStatementNode();
                    stmts.Add(stmt);
                }

                return new ForNode(expr, new BlockNode(stmts));
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
                    return new CallNode(token.Value, expressions);
                }

                if (this.TryParseToken(TokenType.Delimiter, "."))
                {
                    string name2 = this.ParseName();
                    return new QualifiedNameNode(token.Value, name2);
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
