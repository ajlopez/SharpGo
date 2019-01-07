namespace SharpGo.Core.Ast
{
    public class ReturnNode : IStatementNode
    {
        private readonly IExpressionNode expression;

        public ReturnNode(IExpressionNode expression)
        {
            this.expression = expression;
        }

        public IExpressionNode Expression { get { return this.expression; } }
    }
}
