namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class IfNode : IStatementNode
    {
        private INode statement;
        private IExpressionNode expression;
        private BlockNode block;

        public IfNode(IExpressionNode expression, BlockNode block)
            : this(null, expression, block)
        {
        }

        public IfNode(INode statement, IExpressionNode expression, BlockNode block)
        {
            this.statement = statement;
            this.expression = expression;
            this.block = block;
        }

        public INode Statement { get { return this.statement; } }

        public INode Expression { get { return this.expression; } }

        public BlockNode BlockNode { get { return this.block; } }
    }
}
