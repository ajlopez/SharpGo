namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;
    using SharpGo.Core.Language.TypeInfos;

    public class CallNode : IExpressionNode
    {
        private IExpressionNode expression;
        private IList<IExpressionNode> arguments;

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
