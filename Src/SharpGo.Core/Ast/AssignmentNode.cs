namespace SharpGo.Core.Ast
{
    public class AssignmentNode : IStatementNode
    {
        private readonly AssignmentOperator oper;
        private readonly INode target;
        private readonly IExpressionNode expression;

        public AssignmentNode(INode target, IExpressionNode expression)
            : this(AssignmentOperator.Set, target, expression)
        {
        }

        public AssignmentNode(AssignmentOperator oper, INode target, IExpressionNode expression)
        {
            this.oper = oper;
            this.target = target;
            this.expression = expression;
        }

        public AssignmentOperator Operator { get { return this.oper; } }

        public INode TargetNode { get { return this.target; } }

        public IExpressionNode ExpressionNode { get { return this.expression; } }
    }
}
