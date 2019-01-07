namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ForNode : IStatementNode
    {
        private readonly IStatementNode initstmt;
        private readonly IExpressionNode expression;
        private readonly IStatementNode poststmt;
        private readonly IStatementNode body;

        public ForNode(IExpressionNode expression, IStatementNode body)
            : this(null, expression, null, body)
        {
        }

        public ForNode(IStatementNode initstmt, IExpressionNode expression, IStatementNode poststmt, IStatementNode body)
        {
            this.initstmt = initstmt;
            this.expression = expression;
            this.poststmt = poststmt;
            this.body = body;
        }

        public IStatementNode InitStatement { get { return this.initstmt; } }

        public IExpressionNode Expression { get { return this.expression; } }

        public IStatementNode PostStatement { get { return this.poststmt; } }

        public IStatementNode BodyNode { get { return this.body; } }
    }
}
