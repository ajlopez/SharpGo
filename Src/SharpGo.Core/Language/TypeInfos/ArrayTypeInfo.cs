namespace SharpGo.Core.Language.TypeInfos
{
    using System;
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

        public Array NewInstance()
        {
            return Array.CreateInstance(this.TypeInfo.NativeType, (int)((ConstantNode)this.lexpr).Value);
        }
    }
}
