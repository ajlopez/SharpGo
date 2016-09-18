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
        public void GetByteTypeInfo()
        {
            var result = TypeInfo.GetTypeInfo((byte)42);

            Assert.IsNotNull(result);
            Assert.AreSame(TypeInfo.Byte, result);
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

        [TestMethod]
        public void SameTypesAreAssignable()
        {
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Byte, TypeInfo.Byte));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Bool, TypeInfo.Bool));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Complex128, TypeInfo.Complex128));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Complex64, TypeInfo.Complex64));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Float32, TypeInfo.Float32));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Float64, TypeInfo.Float64));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Int, TypeInfo.Int));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Int8, TypeInfo.Int8));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Int16, TypeInfo.Int16));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Int32, TypeInfo.Int32));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Int64, TypeInfo.Int64));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.String, TypeInfo.String));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.UInt, TypeInfo.UInt));
        }

        [TestMethod]
        public void SmallIntegersAreAssignableToBigIntegers()
        {
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Int8, TypeInfo.Int16));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Int8, TypeInfo.Int32));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Int8, TypeInfo.Int64));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Int16, TypeInfo.Int32));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Int16, TypeInfo.Int64));
            Assert.IsTrue(TypeInfo.AreAssignable(TypeInfo.Int32, TypeInfo.Int64));
        }
    }
}
