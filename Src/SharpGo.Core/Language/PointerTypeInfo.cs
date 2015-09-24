namespace SharpGo.Core.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class PointerTypeInfo : TypeInfo
    {
        private TypeInfo typeinfo;

        public PointerTypeInfo(TypeInfo typeinfo)
            : base("pointer")
        {
            this.typeinfo = typeinfo;
        }

        public TypeInfo TypeInfo { get { return this.typeinfo; } }
    }
}
