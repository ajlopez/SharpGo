namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ForNode : IStatementNode
    {
        private IStatementNode initstmt;
        private IExpressionNode expression;
        private IStatementNode poststmt;
        private BlockNode block;

        public ForNode(IExpressionNode expression, BlockNode block)
            : this(null, expression, null, block)
        {
        }

        public ForNode(IStatementNode initstmt, IExpressionNode expression, IStatementNode poststmt, BlockNode block)
        {
            this.initstmt = initstmt;
            this.expression = expression;
            this.poststmt = poststmt;
            this.block = block;
        }

        public IStatementNode InitStatement { get { return this.initstmt; } }

        public IExpressionNode Expression { get { return this.expression; } }

        public IStatementNode PostStatement { get { return this.poststmt; } }

        public BlockNode BlockNode { get { return this.block; } }
    }
}
