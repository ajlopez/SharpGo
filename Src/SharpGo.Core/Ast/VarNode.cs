namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class VarNode : INode
    {
        private IExpressionNode expression;
        private string name;

        public VarNode(string name, IExpressionNode expression)
        {
            this.name = name;
            this.expression = expression;
        }

        public string Name { get { return this.name; } }

        public IExpressionNode ExpressionNode { get { return this.expression; } }
    }
}
