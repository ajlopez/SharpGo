namespace SharpGo.Core.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SliceTypeInfo : TypeInfo
    {
        private TypeInfo typeinfo;

        public SliceTypeInfo(TypeInfo typeinfo)
            : base("slice")
        {
            this.typeinfo = typeinfo;
        }

        public TypeInfo TypeInfo { get { return this.typeinfo; } }
    }
}
