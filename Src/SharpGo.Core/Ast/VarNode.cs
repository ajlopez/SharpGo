namespace SharpGo.Core.Ast
{
    using SharpGo.Core.Language.TypeInfos;

    public class VarNode : IStatementNode
    {
        private readonly string name;
        private readonly TypeInfo typeinfo;
        private readonly IExpressionNode expression;

        public VarNode(string name, TypeInfo typeinfo, IExpressionNode expression)
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

