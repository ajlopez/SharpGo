namespace SharpGo.Core.Language
{
    using SharpGo.Core.Ast;

    public class ArrayTypeInfo : SliceTypeInfo
    {
        private IExpressionNode lexpr;

        public ArrayTypeInfo(TypeInfo typeinfo, IExpressionNode lexpr)
            : base(typeinfo)
        {
            this.lexpr = lexpr;
        }

        public IExpressionNode LengthExpression { get { return this.lexpr; } }
    }
}
