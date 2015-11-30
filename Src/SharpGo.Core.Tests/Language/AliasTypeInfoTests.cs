namespace SharpGo.Core.Tests.Language
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SharpGo.Core.Language;

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
