namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class IfNode : IStatementNode
    {
        private IStatementNode statement;
        private IExpressionNode expression;
        private IStatementNode thenCommand;
        private IStatementNode elseCommand;

        public IfNode(IExpressionNode expression, IStatementNode block)
            : this(null, expression, block)
        {
        }

        public IfNode(IStatementNode statement, IExpressionNode expression, IStatementNode thenNode)
            : this(statement, expression, thenNode, null)
        {
        }

        public IfNode(IStatementNode statement, IExpressionNode expression, IStatementNode thenNode, IStatementNode elseNode)
        {
            this.statement = statement;
            this.expression = expression;
            this.thenCommand = thenNode;
            this.elseCommand = elseNode;
        }

        public IStatementNode Statement { get { return this.statement; } }

        public INode Expression { get { return this.expression; } }

        public IStatementNode ThenCommand { get { return this.thenCommand; } }

        public IStatementNode ElseCommand { get { return this.elseCommand; } }
    }
}
