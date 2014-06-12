namespace SharpGo.Core.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Ast;

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
            var token = this.NextToken();

            while (token != null && token.Type == TokenType.NewLine)
                token = this.NextToken();

            if (token == null)
                return null;

            this.PushToken(token);

            if (this.TryParseToken(TokenType.Delimiter, "{")) {
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

                if (this.TryParseToken(TokenType.Operator, "="))
                    expr = this.ParseExpressionNode();

                this.ParseEndOfStatement();

                return new VarNode(name, expr);
            }

            if (this.TryParseToken(TokenType.Name, "if"))
            {
                var expr = this.ParseExpressionNode();
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
                this.ParseEndOfStatement();

                return new ReturnNode(expr);
            }

            if (this.TryParseToken(TokenType.Name, "defer"))
            {
                var expr = this.ParseExpressionNode();
                this.ParseEndOfStatement();

                return new DeferNode(expr);
            }

            if (this.TryParseToken(TokenType.Name, "go"))
            {
                var expr = this.ParseExpressionNode();
                this.ParseEndOfStatement();

                return new GoNode(expr);
            }

            if (this.TryParseToken(TokenType.Name, "break"))
            {
                string label = this.TryParseName();
                this.ParseEndOfStatement();

                return new BreakNode(label);
            }

            if (this.TryParseToken(TokenType.Name, "continue"))
            {
                string label = this.TryParseName();
                this.ParseEndOfStatement();

                return new ContinueNode(label);
            }

            if (this.TryParseToken(TokenType.Name, "goto"))
            {
                string label = this.ParseName();
                this.ParseEndOfStatement();

                return new GotoNode(label);
            }

            if (this.TryParseToken(TokenType.Name, "func"))
            {
                var name = this.ParseName();
                this.ParseToken(TokenType.Delimiter, "(");
                this.ParseToken(TokenType.Delimiter, ")");

                this.EnsureToken(TokenType.Delimiter, "{");

                var body = this.ParseStatementNode();

                this.ParseEndOfStatement();

                return new FuncNode(name, body);
            }

            INode node = this.ParseExpressionNode();

            if (node == null)
                return null;

            if (this.TryParseToken(TokenType.Operator, "="))
            {
                node = new AssignmentNode(node, this.ParseExpressionNode());
                this.ParseEndOfStatement();
                return node;
            }

            if (this.TryParseToken(TokenType.Operator, "<-"))
            {
                node = new SendNode(node, this.ParseExpressionNode());
                this.ParseEndOfStatement();
                return node;
            }

            node = new ExpressionStatementNode(node);
            this.ParseEndOfStatement();
            return node;
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
