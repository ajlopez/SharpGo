namespace SharpGo.Core.Ast
{
    using SharpGo.Core.Language.TypeInfos;

    public class StructMemberNode : IExpressionNode
    {
        private readonly string name;
        private readonly TypeInfo typeinfo;

        public StructMemberNode(string name, TypeInfo typeinfo)
        {
            this.name = name;
            this.typeinfo = typeinfo;
        }

        public string Name { get { return this.name; } }

        public TypeInfo TypeInfo { get { return this.typeinfo; } }
    }
}
