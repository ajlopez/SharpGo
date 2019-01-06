namespace SharpGo.Core.Tests.Language.TypeInfos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SharpGo.Core.Language;
    using SharpGo.Core.Language.TypeInfos;

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
