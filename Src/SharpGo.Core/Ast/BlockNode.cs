namespace SharpGo.Core.Ast
{
    using System.Collections.Generic;

    public class BlockNode : IStatementNode
    {
        private readonly IList<IStatementNode> statements;

        public BlockNode(IList<IStatementNode> statements)
        {
            this.statements = statements;
        }

        public IList<IStatementNode> Statements { get { return this.statements; } }
    }
}
