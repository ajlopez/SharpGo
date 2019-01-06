namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;
    using SharpGo.Core.Language.TypeInfos;

    public class ListNode : IExpressionNode
    {
        private List<IExpressionNode> expressions;

        public ListNode(List<IExpressionNode> expressions)
        {
            this.expressions = expressions;
        }

        public List<IExpressionNode> Expressions { get { return this.expressions; } }

        public TypeInfo TypeInfo { get { throw new NotImplementedException(); } }
    }
}
