namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;

    public class DotNode : IExpressionNode
    {
        private IExpressionNode expression;
        private string name;

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
