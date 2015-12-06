namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class BlockNode : IStatementNode
    {
        private IList<INode> statements;

        public BlockNode(IList<INode> statements)
        {
            this.statements = statements;
        }

        public IList<INode> Statements { get { return this.statements; } }
    }
}
