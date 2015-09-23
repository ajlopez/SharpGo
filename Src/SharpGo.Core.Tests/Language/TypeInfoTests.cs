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

        [TestMethod]
        public void GetNilTypeInfo()
        {
            var result = TypeInfo.GetTypeInfo(null);

            Assert.IsNotNull(result);
            Assert.AreSame(TypeInfo.Nil, result);
        }

        [TestMethod]
        public void GetInt16TypeInfo()
        {
            var result = TypeInfo.GetTypeInfo((short)42);

            Assert.IsNotNull(result);
            Assert.AreSame(TypeInfo.Int16, result);
        }

        [TestMethod]
        public void GetInt32TypeInfo()
        {
            var result = TypeInfo.GetTypeInfo(42);

            Assert.IsNotNull(result);
            Assert.AreSame(TypeInfo.Int32, result);
        }

        [TestMethod]
        public void GetFloat32TypeInfo()
        {
            var result = TypeInfo.GetTypeInfo((float)3.14);

            Assert.IsNotNull(result);
            Assert.AreSame(TypeInfo.Float32, result);
        }

        [TestMethod]
        public void GetInt64TypeInfo()
        {
            var result = TypeInfo.GetTypeInfo((long)42);

            Assert.IsNotNull(result);
            Assert.AreSame(TypeInfo.Int64, result);
        }
    }
}
