namespace SharpGo.Core.Tests.Language
{
    using System;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SharpGo.Core.Ast;
    using SharpGo.Core.Language;

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
