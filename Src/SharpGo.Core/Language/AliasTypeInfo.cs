namespace SharpGo.Core.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class AliasTypeInfo : TypeInfo
    {
        private TypeInfo typeinfo;

        public AliasTypeInfo(string name, TypeInfo typeinfo)
            : base(name)
        {
            this.typeinfo = typeinfo;
        }

        public TypeInfo TypeInfo { get { return this.typeinfo; } }
    }
}
