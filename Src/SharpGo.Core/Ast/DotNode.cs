namespace SharpGo.Core.Ast
{
    using System;
    using SharpGo.Core.Language.TypeInfos;

    public class DotNode : IExpressionNode
    {
        private readonly IExpressionNode expression;
        private readonly string name;

        public DotNode(IExpressionNode expression, string name)
        {
            this.expression = expression;
            this.name = name;
        }

        public IExpressionNode ExpressionNode { get { return this.expression; } }

        public string Name { get { return this.name; } }

        public TypeInfo TypeInfo
        {
            get { throw new NotImplementedException(); }
        }
    }
}
