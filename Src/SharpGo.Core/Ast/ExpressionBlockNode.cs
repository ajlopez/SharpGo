namespace SharpGo.Core.Ast
{
    using System.Collections.Generic;
    using SharpGo.Core.Language.TypeInfos;

    public class ExpressionBlockNode : IExpressionNode
    {
        private readonly IList<IExpressionNode> expressions;
        private readonly TypeInfo typeinfo;

        public ExpressionBlockNode(IList<IExpressionNode> expressions)
        {
            this.expressions = expressions;
            this.typeinfo = new ArrayTypeInfo(expressions[0].TypeInfo, new ConstantNode(expressions.Count));
        }

        public IList<IExpressionNode> Expressions { get { return this.expressions; } }

        public TypeInfo TypeInfo { get { return this.typeinfo; } }
    }
}
