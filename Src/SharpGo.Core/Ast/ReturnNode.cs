namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ReturnNode : INode
    {
        private INode expression;

        public ReturnNode(INode expression)
        {
            this.expression = expression;
        }

        public INode Expression { get { return this.expression; } }
    }
}
