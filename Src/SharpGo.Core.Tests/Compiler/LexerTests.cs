namespace SharpGo.Core.Tests.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SharpGo.Core.Compiler;

    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void GetName()
        {
            Lexer lexer = new Lexer("name");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Name, token.Type);
            Assert.AreEqual("name", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetNameWithInitialUnderscore()
        {
            Lexer lexer = new Lexer("_name");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Name, token.Type);
            Assert.AreEqual("_name", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetNameWithDigits()
        {
            Lexer lexer = new Lexer("name123");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Name, token.Type);
            Assert.AreEqual("name123", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetNameWithSpaces()
        {
            Lexer lexer = new Lexer("  name   ");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Name, token.Type);
            Assert.AreEqual("name", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetTwoNames()
        {
            Lexer lexer = new Lexer("foo bar");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Name, token.Type);
            Assert.AreEqual("foo", token.Value);

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Name, token.Type);
            Assert.AreEqual("bar", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetInteger()
        {
            Lexer lexer = new Lexer("42");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Integer, token.Type);
            Assert.AreEqual("42", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetReal()
        {
            Lexer lexer = new Lexer("12.34");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Real, token.Type);
            Assert.AreEqual("12.34", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetDotAsDelimiter()
        {
            Lexer lexer = new Lexer(".");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Delimiter, token.Type);
            Assert.AreEqual(".", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetSemicolonAsDelimiter()
        {
            Lexer lexer = new Lexer(";");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Delimiter, token.Type);
            Assert.AreEqual(";", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetCommaAsDelimiter()
        {
            Lexer lexer = new Lexer(",");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Delimiter, token.Type);
            Assert.AreEqual(",", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetPlusAsOperator()
        {
            Lexer lexer = new Lexer("+");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Operator, token.Type);
            Assert.AreEqual("+", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetMinusAsOperator()
        {
            Lexer lexer = new Lexer("-");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Operator, token.Type);
            Assert.AreEqual("-", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetEqualAsAssignmentOperator()
        {
            Lexer lexer = new Lexer("=");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Operator, token.Type);
            Assert.AreEqual("=", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetLeftArrowAsSendOperator()
        {
            Lexer lexer = new Lexer("<-");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Operator, token.Type);
            Assert.AreEqual("<-", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetArithmeticOperators()
        {
            GetOperators(new string[] { "+", "-", "*", "/", "%" });
        }

        [TestMethod]
        public void GetIncrementalOperators()
        {
            GetOperators(new string[] { "++", "--" });
        }

        [TestMethod]
        public void GetComparisonOperators()
        {
            GetOperators(new string[] { "==", "!=", "<", ">", "<=", ">=" });
        }

        [TestMethod]
        public void GetBitwiseArithmeticOperators()
        {
            GetOperators(new string[] { "&", "|", "^", "&^", "<<", ">>" });
        }

        [TestMethod]
        public void GetLogicalOperators()
        {
            GetOperators(new string[] { "&&", "||", "!" });
        }

        [TestMethod]
        public void GetReceiveOperator()
        {
            GetOperators(new string[] { "<-" });
        }

        [TestMethod]
        public void GetAssigmentWithDeclarationOperator()
        {
            GetOperators(new string[] { ":=" });
        }

        [TestMethod]
        public void GetString()
        {
            Lexer lexer = new Lexer("\"foo\"");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.String, token.Type);
            Assert.AreEqual("foo", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetBracketsAsDelimiters()
        {
            Lexer lexer = new Lexer("{}");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Delimiter, token.Type);
            Assert.AreEqual("{", token.Value);

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Delimiter, token.Type);
            Assert.AreEqual("}", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetParenthesisAsDelimiters()
        {
            Lexer lexer = new Lexer("()");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Delimiter, token.Type);
            Assert.AreEqual("(", token.Value);

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Delimiter, token.Type);
            Assert.AreEqual(")", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void UnclosedString()
        {
            Lexer lexer = new Lexer("\"foo");

            try
            {
                var token = lexer.NextToken();
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(LexerException));
                Assert.AreEqual("Unclosed string", ex.Message);
            }
        }

        [TestMethod]
        public void GetNewLine()
        {
            Lexer lexer = new Lexer("\n");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.NewLine, token.Type);
            Assert.AreEqual("\n", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetNewLineWithCarriageReturn()
        {
            Lexer lexer = new Lexer("\r\n");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.NewLine, token.Type);
            Assert.AreEqual("\r\n", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetCarriageReturnAsNewLine()
        {
            Lexer lexer = new Lexer("\r");

            var token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.NewLine, token.Type);
            Assert.AreEqual("\r", token.Value);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void UnexpectedCharacter()
        {
            Lexer lexer = new Lexer("@");

            try
            {
                lexer.NextToken();
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(LexerException));
                Assert.AreEqual("Unexpected '@'", ex.Message);
            }
        }

        private static void GetOperators(IList<string> operators)
        {
            var text = string.Join(" ", operators);
            Lexer lexer = new Lexer(text);

            for (int k = 0; k < operators.Count(); k++)
            {
                var token = lexer.NextToken();

                Assert.IsNotNull(token);
                Assert.AreEqual(TokenType.Operator, token.Type);
                Assert.AreEqual(operators[k], token.Value);
            }

            Assert.IsNull(lexer.NextToken());
        }

        private static void GetDelimiters(IList<string> delimiters)
        {
            var text = string.Join(" ", delimiters);
            Lexer lexer = new Lexer(text);

            for (int k = 0; k < delimiters.Count(); k++)
            {
                var token = lexer.NextToken();

                Assert.IsNotNull(token);
                Assert.AreEqual(TokenType.Delimiter, token.Type);
                Assert.AreEqual(delimiters[k], token.Value);
            }

            Assert.IsNull(lexer.NextToken());
        }
    }
}
