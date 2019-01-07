namespace SharpGo.Core.Ast
{
    using SharpGo.Core.Language.TypeInfos;

    public class ConstantNode : IExpressionNode
    {
        private readonly TypeInfo typeinfo;
        private readonly object value;

        public ConstantNode(object value)
        {
            this.value = value;
            this.typeinfo = TypeInfo.GetTypeInfo(value);
        }

        public object Value { get { return this.value; } }

        public TypeInfo TypeInfo { get { return this.typeinfo; } }
    }
}
