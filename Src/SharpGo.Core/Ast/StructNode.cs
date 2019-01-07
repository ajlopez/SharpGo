namespace SharpGo.Core.Ast
{
    using System.Collections.Generic;

    public class StructNode : IStatementNode
    {
        private readonly IList<StructMemberNode> members;

        public StructNode(IList<StructMemberNode> members)
        {
            this.members = members;
        }

        public IList<StructMemberNode> Members { get { return this.members; } }
    }
}
