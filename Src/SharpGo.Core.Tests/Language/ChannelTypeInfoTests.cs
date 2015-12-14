namespace SharpGo.Core.Tests.Language
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
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
