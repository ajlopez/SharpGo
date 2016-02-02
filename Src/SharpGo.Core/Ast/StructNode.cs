namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;

    public class StructNode : IStatementNode
    {
        IList<StructMemberNode> members;

        public StructNode(IList<StructMemberNode> members)
        {
            this.members = members;
        }

        public IList<StructMemberNode> Members { get { return this.members; } }
    }
}
