namespace SharpGo.Core.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class MapTypeInfo : TypeInfo
    {
        private TypeInfo keytypeinfo;
        private TypeInfo elementtypeinfo;

        public MapTypeInfo(TypeInfo keytypeinfo, TypeInfo elementtypeinfo)
            : base("map")
        {
            this.keytypeinfo = keytypeinfo;
            this.elementtypeinfo = elementtypeinfo;
        }

        public TypeInfo KeyTypeInfo { get { return this.keytypeinfo; } }

        public TypeInfo ElementTypeInfo { get { return this.elementtypeinfo; } }
    }
}
