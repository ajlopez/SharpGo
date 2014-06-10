namespace SharpGo.Core.Tests.Language
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SharpGo.Core.Language;

    [TestClass]
    public class TypeInfoTests
    {
        [TestMethod]
        public void GetBoolTypeInfoFromTrue()
        {
            var result = TypeInfo.GetTypeInfo(true);

            Assert.IsNotNull(result);
            Assert.AreSame(TypeInfo.Bool, result);
        }

        [TestMethod]
        public void GetStringTypeInfo()
        {
            var result = TypeInfo.GetTypeInfo("foo");

            Assert.IsNotNull(result);
            Assert.AreSame(TypeInfo.String, result);
        }
    }
}
