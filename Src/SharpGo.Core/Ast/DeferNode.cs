namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class DeferNode : INode
    {
        private INode expression;

        public DeferNode(INode expression)
        {
            this.expression = expression;
        }

        public INode ExpressionNode { get { return this.expression; } }
    }
}
