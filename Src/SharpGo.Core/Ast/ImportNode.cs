namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ImportNode : INode
    {
        private INode expression;

        public ImportNode(INode expression)
        {
            this.expression = expression;
        }

        public INode ExpressionNode { get { return this.expression; } }
    }
}
