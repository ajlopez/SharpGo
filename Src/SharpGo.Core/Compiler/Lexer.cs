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

            char ch = this.text[this.position];

            if (IsLetter(ch))
                return this.NextName(ch);

            if (IsDigit(ch))
                return this.NextInteger(ch);

            throw new LexerException(string.Format("Unexpected '{0}'", ch));
        }

        private Token NextName(char ich)
        {
            string value = ich.ToString();

            while (++this.position < this.length)
            {
                char ch = this.text[this.position];

                if (!IsLetter(ch) && !IsDigit(ch))
                    break;

                value += ch;
            }

            return new Token(TokenType.Name, value);
        }

        private Token NextInteger(char ich)
        {
            string value = ich.ToString();

            while (++this.position < this.length)
            {
                char ch = this.text[this.position];

                if (!IsDigit(ch))
                    break;

                value += ch;
            }

            return new Token(TokenType.Integer, value);
        }

        private static bool IsLetter(char ch)
        {
            return ch == '_' || char.IsLetter(ch);
        }

        private static bool IsDigit(char ch)
        {
            return char.IsDigit(ch);
        }
    }
}
