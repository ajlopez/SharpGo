namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class DeferNode : INode
    {
        private IExpressionNode expression;

        public DeferNode(IExpressionNode expression)
        {
            this.expression = expression;
        }

        public IExpressionNode ExpressionNode { get { return this.expression; } }
    }
}
