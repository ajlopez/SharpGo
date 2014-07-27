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
            IExpressionNode term = this.ParseTerm();

            if (term == null)
                return null;

            var token = this.NextToken();

            if (token == null)
                return term;

            if (token.Type == TokenType.Operator)
                if (token.Value == "+")
                    return new BinaryNode(term, BinaryOperator.Add, this.ParseTerm());
                else if (token.Value == "-")
                    return new BinaryNode(term, BinaryOperator.Substract, this.ParseTerm());
                else if (token.Value == "*")
                    return new BinaryNode(term, BinaryOperator.Multiply, this.ParseTerm());
                else if (token.Value == "/")
                    return new BinaryNode(term, BinaryOperator.Divide, this.ParseTerm());
                else if (token.Value == "==")
                    return new BinaryNode(term, BinaryOperator.Equal, this.ParseTerm());
                else if (token.Value == "!=")
                    return new BinaryNode(term, BinaryOperator.NotEqual, this.ParseTerm());
                else if (token.Value == "<")
                    return new BinaryNode(term, BinaryOperator.Less, this.ParseTerm());
                else if (token.Value == ">")
                    return new BinaryNode(term, BinaryOperator.Greater, this.ParseTerm());
                else if (token.Value == "<=")
                    return new BinaryNode(term, BinaryOperator.LessEqual, this.ParseTerm());
                else if (token.Value == ">=")
                    return new BinaryNode(term, BinaryOperator.GreaterEqual, this.ParseTerm());

            this.PushToken(token);

            return term;
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

            if (this.TryParseToken(TokenType.Name, "func"))
            {
                var name = this.ParseName();
                this.ParseToken(TokenType.Delimiter, "(");
                this.ParseToken(TokenType.Delimiter, ")");

                this.EnsureToken(TokenType.Delimiter, "{");

                var body = this.ParseStatementNode();

                return new FuncNode(name, body);
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

            if (token.Type == TokenType.String)
                return new ConstantNode(token.Value);

            if (token.Type == TokenType.Name)
            {
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
