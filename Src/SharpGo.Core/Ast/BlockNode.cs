namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class BlockNode : IStatementNode
    {
        private IList<IStatementNode> statements;

        public BlockNode(IList<IStatementNode> statements)
        {
            this.statements = statements;
        }

        public IList<IStatementNode> Statements { get { return this.statements; } }
    }
}
