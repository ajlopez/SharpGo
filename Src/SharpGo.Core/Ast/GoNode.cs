namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class GoNode : INode
    {
        private INode expression;

        public GoNode(INode expression)
        {
            this.expression = expression;
        }

        public INode ExpressionNode { get { return this.expression; } }
    }
}
