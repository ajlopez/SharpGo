namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class GoNode : INode
    {
        private IExpressionNode expression;

        public GoNode(IExpressionNode expression)
        {
            this.expression = expression;
        }

        public INode ExpressionNode { get { return this.expression; } }
    }
}
