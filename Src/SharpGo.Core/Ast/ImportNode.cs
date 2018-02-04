namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ImportNode : IStatementNode
    {
        private IExpressionNode expression;

        public ImportNode(IExpressionNode expression)
        {
            this.expression = expression;
        }

        public IExpressionNode ExpressionNode { get { return this.expression; } }
    }
}
