namespace SharpGo.Core.Tests.Language
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SharpGo.Core.Language;

    [TestClass]
    public class TypeArrayTests
    {
        [TestMethod]
        public void CreateIntegerArray()
        {
            var array = new TypedArray<int>(10);

            Assert.IsNotNull(array);
            Assert.AreEqual(10, array.Length);

            for (int k = 0; k < array.Length; k++)
                Assert.AreEqual(0, array[k]);
        }

        [TestMethod]
        public void SetAndGetValuesUsingIntegerArray()
        {
            var array = new TypedArray<int>(10);

            for (int k = 0; k < array.Length; k++)
                array[k] = k + 1;

            for (int k = 0; k < array.Length; k++)
                Assert.AreEqual(k + 1, array[k]);
        }
    }
}
