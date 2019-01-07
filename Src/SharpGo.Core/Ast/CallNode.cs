namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using SharpGo.Core.Language.TypeInfos;

    public class CallNode : IExpressionNode
    {
        private readonly IExpressionNode expression;
        private readonly IList<IExpressionNode> arguments;

        public CallNode(IExpressionNode expression, IList<IExpressionNode> arguments)
        {
            this.expression = expression;
            this.arguments = arguments;
        }

        public IExpressionNode ExpressionNode { get { return this.expression; } }

        public IList<IExpressionNode> Arguments { get { return this.arguments; } }

        public TypeInfo TypeInfo
        {
            get { throw new NotImplementedException(); }
        }
    }
}
