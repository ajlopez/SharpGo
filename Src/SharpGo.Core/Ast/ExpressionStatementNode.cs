namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ExpressionStatementNode : IStatementNode
    {
        private readonly IExpressionNode expression;

        public ExpressionStatementNode(IExpressionNode expression)
        {
            this.expression = expression;
        }

        public IExpressionNode ExpressionNode { get { return this.expression; } }
    }
}
