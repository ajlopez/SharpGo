namespace SharpGo.Core.Ast
{
    public class GoNode : IStatementNode
    {
        private readonly IExpressionNode expression;

        public GoNode(IExpressionNode expression)
        {
            this.expression = expression;
        }

        public IExpressionNode ExpressionNode { get { return this.expression; } }
    }
}
