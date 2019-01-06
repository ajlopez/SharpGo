namespace SharpGo.Core.Tests.Language.TypeInfos
{
    using System;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SharpGo.Core.Ast;
    using SharpGo.Core.Language;
    using SharpGo.Core.Language.TypeInfos;

    [TestClass]
    public class ArrayTypeInfoTests
    {
        [TestMethod]
        public void ArrayTypeInfoNewInstance()
        {
            var typeinfo = new ArrayTypeInfo(TypeInfo.String, new ConstantNode(100));

            var instance = typeinfo.NewInstance();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(string[]));
            Assert.AreEqual(100, instance.Length);
        }
    }
}
