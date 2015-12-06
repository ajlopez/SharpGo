namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class BreakNode : IStatementNode
    {
        private string label;

        public BreakNode(string label)
        {
            this.label = label;
        }

        public string Label { get { return this.label; } }
    }
}
