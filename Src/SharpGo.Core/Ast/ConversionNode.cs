namespace SharpGo.Core.Ast
{
    using SharpGo.Core.Language.TypeInfos;

    public class ConversionNode : IExpressionNode
    {
        private readonly IExpressionNode expression;
        private readonly TypeInfo typeinfo;

        public ConversionNode(TypeInfo typeinfo, IExpressionNode expression)
        {
            this.typeinfo = typeinfo;
            this.expression = expression;
        }

        public IExpressionNode ExpressionNode { get { return this.expression; } }

        public TypeInfo TypeInfo { get { return this.typeinfo; } }
    }
}
