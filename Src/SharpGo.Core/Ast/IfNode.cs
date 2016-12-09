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
        private BlockNode thenCommand;
        private BlockNode elseCommand;

        public IfNode(IExpressionNode expression, BlockNode block)
            : this(null, expression, block)
        {
        }

        public IfNode(IStatementNode statement, IExpressionNode expression, BlockNode thenNode)
            : this(statement, expression, thenNode, null)
        {
        }

        public IfNode(IStatementNode statement, IExpressionNode expression, BlockNode thenNode, BlockNode elseNode)
        {
            this.statement = statement;
            this.expression = expression;
            this.thenCommand = thenNode;
            this.elseCommand = elseNode;
        }

        public IStatementNode Statement { get { return this.statement; } }

        public INode Expression { get { return this.expression; } }

        public BlockNode ThenCommand { get { return this.thenCommand; } }

        public BlockNode ElseCommand { get { return this.elseCommand; } }
    }
}
