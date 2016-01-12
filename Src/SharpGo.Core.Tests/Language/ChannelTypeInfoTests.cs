namespace SharpGo.Core.Tests.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SharpGo.Core.Language;

    [TestClass]
    public class ChannelTypeInfoTests
    {
        [TestMethod]
        public void CreateWithSameType()
        {
            var type = new ChannelTypeInfo(TypeInfo.Int);

            Assert.AreSame(TypeInfo.Int, type.ReceiveTypeInfo);
            Assert.AreSame(TypeInfo.Int, type.SendTypeInfo);
        }
    }
}
