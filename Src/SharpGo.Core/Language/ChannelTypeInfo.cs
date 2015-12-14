namespace SharpGo.Core.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ChannelTypeInfo : TypeInfo
    {
        private TypeInfo sendtypeinfo;
        private TypeInfo receivetypeinfo;

        public ChannelTypeInfo(TypeInfo typeinfo)
            : base("channel")
        {
            this.sendtypeinfo = typeinfo;
            this.receivetypeinfo = typeinfo;
        }

        public TypeInfo ReceiveTypeInfo { get { return this.receivetypeinfo; } }

        public TypeInfo SendTypeInfo { get { return this.sendtypeinfo; } }
    }
}
