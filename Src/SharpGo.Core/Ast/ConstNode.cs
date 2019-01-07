namespace SharpGo.Core.Ast
{
    using SharpGo.Core.Language.TypeInfos;

    public class ConstNode : IStatementNode
    {
        private readonly IExpressionNode expression;
        private readonly string name;
        private readonly TypeInfo typeinfo;

        public ConstNode(string name, TypeInfo typeinfo, IExpressionNode expression)
        {
            this.name = name;
            this.typeinfo = typeinfo;
            this.expression = expression;

            if (typeinfo == null)
                this.typeinfo = this.expression.TypeInfo;
        }

        public string Name { get { return this.name; } }

        public IExpressionNode ExpressionNode { get { return this.expression; } }

        public TypeInfo TypeInfo { get { return this.typeinfo; } }
    }
}
