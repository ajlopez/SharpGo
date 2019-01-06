namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;
    using SharpGo.Core.Language.TypeInfos;

    public class BinaryNode : IExpressionNode
    {
        private IExpressionNode leftnode;
        private IExpressionNode rightnode;
        private BinaryOperator @operator;

        public BinaryNode(IExpressionNode leftnode, BinaryOperator @operator, IExpressionNode rightnode)
        {
            this.leftnode = leftnode;
            this.@operator = @operator;
            this.rightnode = rightnode;
        }

        public IExpressionNode LeftNode { get { return this.leftnode; } }

        public IExpressionNode RightNode { get { return this.rightnode; } }

        public BinaryOperator Operator { get { return this.@operator; } }

        public TypeInfo TypeInfo
        {
            get 
            {
                if (this.LeftNode.TypeInfo.Equals(this.RightNode.TypeInfo))
                    return this.LeftNode.TypeInfo;

                throw new NotImplementedException(); 
            }
        }
    }
}
