namespace SharpGo.Core.Ast
{
    using System;
    using SharpGo.Core.Language.TypeInfos;

    public class IndexedNode : IExpressionNode
    {
        private readonly string name;
        private readonly IExpressionNode index;

        public IndexedNode(string name, IExpressionNode index)
        {
            this.name = name;
            this.index = index;
        }

        public string Name { get { return this.name; } }

        public IExpressionNode Index { get { return this.index; } }

        public TypeInfo TypeInfo
        {
            get { throw new NotImplementedException(); }
        }
    }
}
