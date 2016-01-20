namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;

    public class ConversionNode : IExpressionNode
    {
        private IExpressionNode expression;
        private TypeInfo typeinfo;

        public ConversionNode(TypeInfo typeinfo, IExpressionNode expression)
        {
            this.typeinfo = typeinfo;
            this.expression = expression;
        }

        public IExpressionNode ExpressionNode { get { return this.expression; } }

        public TypeInfo TypeInfo { get { return this.typeinfo; } }
    }
}
