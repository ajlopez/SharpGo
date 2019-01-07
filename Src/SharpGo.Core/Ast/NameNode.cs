namespace SharpGo.Core.Ast
{
    using System;
    using SharpGo.Core.Language.TypeInfos;

    public class NameNode : IExpressionNode
    {
        private readonly string name;

        public NameNode(string name)
        {
            this.name = name;
        }

        public string Name { get { return this.name; } }

        public TypeInfo TypeInfo
        {
            get { throw new NotImplementedException(); }
        }
    }
}
