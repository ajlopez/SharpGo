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
        public void ParseNameConstant()
        {
            Parser parser = new Parser("foo");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(NameNode));
            Assert.AreEqual("foo", ((NameNode)node).Name);

            Assert.IsNull(parser.ParseExpressionNode());
        }
    }
}
