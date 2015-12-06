namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class LabelNode : IStatementNode
    {
        private string label;
        private IStatementNode statement;

        public LabelNode(string label, IStatementNode statement)
        {
            this.label = label;
            this.statement = statement;
        }

        public string Label { get { return this.label; } }

        public IStatementNode StatementNode { get { return this.statement; } }
    }
}
