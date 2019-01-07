namespace SharpGo.Core.Ast
{
    public class IfNode : IStatementNode
    {
        private readonly IStatementNode statement;
        private readonly IExpressionNode expression;
        private readonly IStatementNode thenCommand;
        private readonly IStatementNode elseCommand;

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
