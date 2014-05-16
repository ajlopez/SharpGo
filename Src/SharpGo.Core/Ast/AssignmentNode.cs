namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class AssignmentNode : INode
    {
        private INode target;
        private INode expression;

        public AssignmentNode(INode target, INode expression)
        {
            this.target = target;
            this.expression = expression;
        }

        public INode TargetNode { get { return this.target; } }

        public INode ExpressionNode { get { return this.expression; } }
    }
}
