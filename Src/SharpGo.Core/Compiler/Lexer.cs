namespace SharpGo.Core.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Lexer
    {
        private static string delimiters = ";.";
        private static string[] operators = new string[] { "+", "-", "*", "/", "==", "!=", "<", ">", "<=", ">=" };

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

            char ch = this.text[this.position++];

            if (IsLetter(ch))
                return this.NextName(ch);

            if (IsDigit(ch))
                return this.NextInteger(ch);

            if (ch == '"')
                return this.NextString();

            if (delimiters.Contains(ch))
                return new Token(TokenType.Delimiter, ch.ToString());

            var str = ch.ToString();

            if (this.position < this.length)
            {
                var str2 = str + this.text[this.position].ToString();

                if (operators.Contains(str2))
                {
                    this.position++;
                    return new Token(TokenType.Operator, str2);
                }
            }

            if (operators.Contains(str))
                return new Token(TokenType.Operator, str);

            throw new LexerException(string.Format("Unexpected '{0}'", ch));
        }

        private Token NextString()
        {
            string value = string.Empty;
            bool closed = false;

            while (this.position < this.length)
            {
                var ch = this.text[this.position++];

                if (ch == '"')
                {
                    closed = true;
                    break;
                }

                value += ch;
            }

            if (!closed)
                throw new LexerException("Unclosed string");

            return new Token(TokenType.String, value);
        }

        private Token NextName(char ich)
        {
            string value = ich.ToString();

            while (this.position < this.length)
            {
                char ch = this.text[this.position++];

                if (!IsLetter(ch) && !IsDigit(ch))
                    break;

                value += ch;
            }

            return new Token(TokenType.Name, value);
        }

        private Token NextInteger(char ich)
        {
            string value = ich.ToString();

            while (this.position < this.length)
            {
                char ch = this.text[this.position];

                if (!IsDigit(ch))
                    break;

                this.position++;
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
