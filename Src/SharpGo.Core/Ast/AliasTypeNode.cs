namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;
    using SharpGo.Core.Language.TypeInfos;

    public class AliasTypeNode : IStatementNode
    {
        private string name;
        private TypeInfo typeinfo;

        public AliasTypeNode(string name, TypeInfo typeinfo)
        {
            this.name = name;
            this.typeinfo = typeinfo;
        }

        public string Name { get { return this.name; } }

        public TypeInfo TypeInfo { get { return this.typeinfo; } }
    }
}
