namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class GotoNode : IStatementNode
    {
        private readonly string label;

        public GotoNode(string label)
        {
            this.label = label;
        }

        public string Label { get { return this.label; } }
    }
}
