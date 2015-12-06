namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ReturnNode : IStatementNode
    {
        private IExpressionNode expression;

        public ReturnNode(IExpressionNode expression)
        {
            this.expression = expression;
        }

        public IExpressionNode Expression { get { return this.expression; } }
    }
}
