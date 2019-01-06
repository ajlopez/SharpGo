namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using SharpGo.Core.Language;
    using SharpGo.Core.Language.TypeInfos;

    public class ExpressionBlockNode : IExpressionNode
    {
        private IList<IExpressionNode> expressions;
        private TypeInfo typeinfo;

        public ExpressionBlockNode(IList<IExpressionNode> expressions)
        {
            this.expressions = expressions;
            this.typeinfo = new ArrayTypeInfo(expressions[0].TypeInfo, new ConstantNode(expressions.Count));
        }

        public IList<IExpressionNode> Expressions { get { return this.expressions; } }

        public TypeInfo TypeInfo { get { return this.typeinfo; } }
    }
}
