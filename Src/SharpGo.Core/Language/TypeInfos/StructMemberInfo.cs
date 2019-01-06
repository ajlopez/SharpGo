namespace SharpGo.Core.Language.TypeInfos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class StructMemberInfo
    {
        private string name;
        private TypeInfo typeinfo;

        public StructMemberInfo(string name, TypeInfo typeinfo)
        {
            this.name = name;
            this.typeinfo = typeinfo;
        }

        public string Name { get { return this.name; } }

        public TypeInfo TypeInfo { get { return this.typeinfo; } }
    }
}
