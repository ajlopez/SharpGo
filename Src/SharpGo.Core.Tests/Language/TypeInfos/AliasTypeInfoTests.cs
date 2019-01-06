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
    public class AliasTypeInfoTests
    {
        [TestMethod]
        public void CreateAliasTypeInfo()
        {
            var result = new AliasTypeInfo("TokenType", TypeInfo.Int);

            Assert.AreEqual("TokenType", result.Name);
            Assert.AreSame(TypeInfo.Int, result.TypeInfo);
        }
    }
}
