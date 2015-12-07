namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ForNode : IStatementNode
    {
        private IExpressionNode expression;
        private BlockNode block;

        public ForNode(IExpressionNode expression, BlockNode block)
        {
            this.expression = expression;
            this.block = block;
        }

        public IExpressionNode Expression { get { return this.expression; } }

        public BlockNode BlockNode { get { return this.block; } }
    }
}
