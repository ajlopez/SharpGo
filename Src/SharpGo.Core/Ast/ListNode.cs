namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using SharpGo.Core.Language.TypeInfos;

    public class ListNode : IExpressionNode
    {
        private readonly List<IExpressionNode> expressions;

        public ListNode(List<IExpressionNode> expressions)
        {
            this.expressions = expressions;
        }

        public List<IExpressionNode> Expressions { get { return this.expressions; } }

        public TypeInfo TypeInfo { get { throw new NotImplementedException(); } }
    }
}
