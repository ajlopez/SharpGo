namespace SharpGo.Core.Tests.Language
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SharpGo.Core.Language;

    [TestClass]
    public class MapTypeInfoTests
    {
        [TestMethod]
        public void CreateWithTypes()
        {
            var type = new MapTypeInfo(TypeInfo.String, TypeInfo.Int);

            Assert.AreSame(TypeInfo.String, type.KeyTypeInfo);
            Assert.AreSame(TypeInfo.Int, type.ElementTypeInfo);
        }
    }
}
