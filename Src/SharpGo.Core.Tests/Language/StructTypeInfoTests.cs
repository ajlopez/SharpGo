namespace SharpGo.Core.Tests.Language
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SharpGo.Core.Language;

    [TestClass]
    public class StructTypeInfoTests
    {
        [TestMethod]
        public void CreateStructMemberInfo()
        {
            var sm = new StructMemberInfo("name", TypeInfo.String);

            Assert.AreEqual("name", sm.Name);
            Assert.AreSame(TypeInfo.String, sm.TypeInfo);
        }
    }
}
