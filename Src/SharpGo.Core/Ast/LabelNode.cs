namespace SharpGo.Core.Ast
{
    public class LabelNode : IStatementNode
    {
        private readonly string label;
        private readonly IStatementNode statement;

        public LabelNode(string label, IStatementNode statement)
        {
            this.label = label;
            this.statement = statement;
        }

        public string Label { get { return this.label; } }

        public IStatementNode StatementNode { get { return this.statement; } }
    }
}
