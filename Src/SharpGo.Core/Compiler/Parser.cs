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
        private Lexer lexer;

        public Parser(string text)
        {
            this.lexer = new Lexer(text);
        }

        public INode ParseExpressionNode()
        {
            INode term = this.ParseTerm();

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

            throw new ParserException(string.Format("Unexpected '{0}'", token.Value));
        }

        private INode ParseTerm()
        {
            var token = this.NextToken();

            if (token == null)
                return null;

            if (token.Type == TokenType.Integer)
                return new ConstantNode(int.Parse(token.Value, CultureInfo.InvariantCulture));

            if (token.Type == TokenType.String)
                return new ConstantNode(token.Value);

            if (token.Type == TokenType.Name)
                return new NameNode(token.Value);

            return null;
        }

        private Token NextToken()
        {
            return this.lexer.NextToken();
        }
    }
}
