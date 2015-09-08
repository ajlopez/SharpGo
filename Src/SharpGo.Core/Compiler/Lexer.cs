namespace SharpGo.Core.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Lexer
    {
        private static string[] delimiters = new string[] { "\"", ";", ":", ".", "{", "}", "(", ")", ",", "...", "[", "]" };
        private static string[] operators = new string[] { "+", "-", "*", "/", "%", "==", "!=", "<", ">", "<=", ">=", "=", "&", "|", "^", "&^", "<<", ">>", "&&", "||", "!", "<-", ":=", "++", "--", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "&^=", "<<=", ">>=" };
        private static Dictionary<char, char> escaped = new Dictionary<char, char>() { { 'n', '\n' }, { 'r', '\r' }, { 't', '\t' }, { '"', '"' }, { '\\', '\\' }, };

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
            while (this.position < this.length && IsWhiteSpace(this.text[this.position]))
                this.position++;

            if (this.position >= this.length)
                return null;

            char ch = this.text[this.position++];

            if (ch == '\n')
                return new Token(TokenType.NewLine, "\n");

            if (ch == '\r')
            {
                if (this.position < this.length && this.text[this.position] == '\n') 
                {
                    this.position++;
                    return new Token(TokenType.NewLine, "\r\n");
                }

                return new Token(TokenType.NewLine, "\r");
            }

            if (IsLetter(ch))
                return this.NextName(ch);

            if (IsDigit(ch))
            {
                if (ch == '0' && this.position < this.length && this.text[this.position] == 'x')
                {
                    this.position++;
                    return this.NextHexadecimalInteger();
                }

                return this.NextInteger(ch);
            }

            if (ch == '"')
                return this.NextString();

            var str = ch.ToString();

            if (this.position < this.length)
            {
                var str2 = str + this.text[this.position].ToString();

                if (this.position + 1 < this.length)
                {
                    var str3 = str2 + this.text[this.position + 1].ToString();

                    if (operators.Contains(str3))
                    {
                        this.position += 2;
                        return new Token(TokenType.Operator, str3);
                    }

                    if (delimiters.Contains(str3))
                    {
                        this.position += 2;
                        return new Token(TokenType.Delimiter, str3);
                    }
                }

                if (operators.Contains(str2))
                {
                    this.position++;
                    return new Token(TokenType.Operator, str2);
                }
            }

            if (delimiters.Contains(str))
                return new Token(TokenType.Delimiter, str);

            if (operators.Contains(str))
                return new Token(TokenType.Operator, str);

            throw new LexerException(string.Format("Unexpected '{0}'", ch));
        }

        private static bool IsLetter(char ch)
        {
            return ch == '_' || char.IsLetter(ch);
        }

        private static bool IsDigit(char ch)
        {
            return char.IsDigit(ch);
        }

        private static bool IsHexadecimalDigit(char ch)
        {
            return IsDigit(ch) || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');
        }

        private static bool IsWhiteSpace(char ch)
        {
            if (ch == '\r' || ch == '\n')
                return false;

            return char.IsWhiteSpace(ch);
        }

        private Token NextString()
        {
            string value = string.Empty;
            bool closed = false;

            while (this.position < this.length)
            {
                var ch = this.text[this.position++];

                if (ch == '\\' && this.position < this.length && escaped.ContainsKey(this.text[this.position]))
                {
                    value += escaped[this.text[this.position]];
                    this.position++;
                    continue;
                }

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
                char ch = this.text[this.position];

                if (!IsLetter(ch) && !IsDigit(ch))
                    break;

                this.position++;
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

            if (this.position < this.length && this.text[this.position] == '.')
            {
                this.position++;
                return this.NextReal(value);
            }

            return new Token(TokenType.Integer, value);
        }

        private Token NextHexadecimalInteger()
        {
            string value = "0x";

            while (this.position < this.length)
            {
                char ch = this.text[this.position];

                if (!IsHexadecimalDigit(ch))
                    break;

                this.position++;
                value += ch;
            }

            return new Token(TokenType.Integer, value);
        }

        private Token NextReal(string integers)
        {
            string value = integers + ".";

            while (this.position < this.length)
            {
                char ch = this.text[this.position];

                if (!IsDigit(ch))
                    break;

                this.position++;
                value += ch;
            }

            return new Token(TokenType.Real, value);
        }
    }
}
