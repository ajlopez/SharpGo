namespace SharpGo.Core.Ast
{
    public class BreakNode : IStatementNode
    {
        private readonly string label;

        public BreakNode(string label)
        {
            this.label = label;
        }

        public string Label { get { return this.label; } }
    }
}
