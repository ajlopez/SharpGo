namespace SharpGo.Core.Tests.Compiler
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SharpGo.Core.Ast;
    using SharpGo.Core.Compiler;

    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ParseIntegerConstant()
        {
            Parser parser = new Parser("123");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ConstantNode));
            Assert.AreEqual(123, ((ConstantNode)node).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseStringConstant()
        {
            Parser parser = new Parser("\"foo\"");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ConstantNode));
            Assert.AreEqual("foo", ((ConstantNode)node).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseName()
        {
            Parser parser = new Parser("foo");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(NameNode));
            Assert.AreEqual("foo", ((NameNode)node).Name);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseAddTwoIntegers()
        {
            Parser parser = new Parser("1+2");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(BinaryNode));

            var bnode = (BinaryNode)node;

            Assert.AreEqual(BinaryOperator.Add, bnode.Operator);
            Assert.IsNotNull(bnode.LeftNode);
            Assert.IsInstanceOfType(bnode.LeftNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)bnode.LeftNode).Value);
            Assert.IsNotNull(bnode.RightNode);
            Assert.IsInstanceOfType(bnode.RightNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)bnode.RightNode).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseSubtractTwoIntegers()
        {
            Parser parser = new Parser("1-2");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(BinaryNode));

            var bnode = (BinaryNode)node;

            Assert.AreEqual(BinaryOperator.Substract, bnode.Operator);
            Assert.IsNotNull(bnode.LeftNode);
            Assert.IsInstanceOfType(bnode.LeftNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)bnode.LeftNode).Value);
            Assert.IsNotNull(bnode.RightNode);
            Assert.IsInstanceOfType(bnode.RightNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)bnode.RightNode).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseUnexpectedInteger()
        {
            Parser parser = new Parser("1 2");

            try
            {
                parser.ParseExpressionNode();
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ParserException));
                Assert.AreEqual("Unexpected '2'", ex.Message);
            }
        }
    }
}
