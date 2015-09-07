namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;

    public class IndexedNode : IExpressionNode
    {
        private string name;
        private IExpressionNode index;

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
