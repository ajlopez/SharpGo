namespace SharpGo.Core.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Lexer
    {
        private string text;
        private int length;
        private int position;

        public Lexer(string text)
        {
            this.text = text;
            this.length = text.Length;
            this.position = 0;
        }

        public Token NextToken()
        {
            while (this.position < this.length && char.IsWhiteSpace(this.text[this.position]))
                this.position++;

            if (this.position >= this.length)
                return null;

            string name = string.Empty;

            while (this.position < this.length && char.IsLetter(this.text[this.position]))
                name += this.text[this.position++];

            return new Token(TokenType.Name, name);
        }
    }
}
