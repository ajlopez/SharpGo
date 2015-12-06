namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ContinueNode : IStatementNode
    {
        private string label;

        public ContinueNode(string label)
        {
            this.label = label;
        }

        public string Label { get { return this.label; } }
    }
}
