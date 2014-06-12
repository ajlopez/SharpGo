namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;

    public class BinaryNode : IExpressionNode
    {
        private INode leftnode;
        private INode rightnode;
        private BinaryOperator @operator;

        public BinaryNode(INode leftnode, BinaryOperator @operator, INode rightnode)
        {
            this.leftnode = leftnode;
            this.@operator = @operator;
            this.rightnode = rightnode;
        }

        public INode LeftNode { get { return this.leftnode; } }

        public INode RightNode { get { return this.rightnode; } }

        public BinaryOperator Operator { get { return this.@operator; } }

        public TypeInfo TypeInfo
        {
            get { throw new NotImplementedException(); }
        }
    }
}
