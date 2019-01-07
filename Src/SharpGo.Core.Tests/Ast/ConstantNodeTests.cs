namespace SharpGo.Core.Tests.Ast
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SharpGo.Core.Ast;
    using SharpGo.Core.Language.TypeInfos;

    [TestClass]
    public class ConstantNodeTests
    {
        [TestMethod]
        public void GetBooleanValueAndType()
        {
            var node = new ConstantNode(true);

            Assert.AreEqual(true, node.Value);
            Assert.AreSame(TypeInfo.Bool, node.TypeInfo);
        }
    }
}
