namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SendNode : INode
    {
        private INode target;
        private IExpressionNode expression;

        public SendNode(INode target, IExpressionNode expression)
        {
            this.target = target;
            this.expression = expression;
        }

        public INode TargetNode { get { return this.target; } }

        public IExpressionNode ExpressionNode { get { return this.expression; } }
    }
}
