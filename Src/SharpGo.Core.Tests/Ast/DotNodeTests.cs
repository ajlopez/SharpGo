namespace SharpGo.Core.Tests.Ast
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SharpGo.Core.Ast;
    using SharpGo.Core.Language;

    [TestClass]
    public class DotNodeTests
    {
        [TestMethod]
        public void CreateDotNode()
        {
            var expr = new NameNode("foo");
            var node = new DotNode(expr, "bar");

            Assert.AreSame(expr, node.ExpressionNode);
            Assert.AreEqual("bar", node.Name);
        }
    }
}
