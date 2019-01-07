namespace SharpGo.Core.Ast
{
    using System;
    using SharpGo.Core.Language.TypeInfos;

    public class UnaryNode : IExpressionNode
    {
        private readonly IExpressionNode exprnode;
        private readonly UnaryOperator @operator;

        public UnaryNode(IExpressionNode exprnode, UnaryOperator @operator)
        {
            this.exprnode = exprnode;
            this.@operator = @operator;
        }

        public IExpressionNode ExpressionNode { get { return this.exprnode; } }

        public UnaryOperator Operator { get { return this.@operator; } }

        public TypeInfo TypeInfo
        {
            get { throw new NotImplementedException(); }
        }
    }
}
