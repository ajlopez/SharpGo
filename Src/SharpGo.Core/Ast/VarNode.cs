namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class VarNode : INode
    {
        private INode expression;
        private string name;

        public VarNode(string name, INode expression)
        {
            this.name = name;
            this.expression = expression;
        }

        public string Name { get { return this.name; } }

        public INode ExpressionNode { get { return this.expression; } }
    }
}
