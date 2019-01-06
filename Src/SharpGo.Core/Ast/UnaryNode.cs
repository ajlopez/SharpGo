namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;
    using SharpGo.Core.Language.TypeInfos;

    public class UnaryNode : IExpressionNode
    {
        private IExpressionNode exprnode;
        private UnaryOperator @operator;

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
