namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class FuncNode : IStatementNode
    {
        private string name;
        private IList<NameNode> parameters;
        private INode body;

        public FuncNode(string name, IList<NameNode> parameters, INode body)
        {
            this.name = name;
            this.parameters = parameters;
            this.body = body;
        }

        public string Name { get { return this.name; } }

        public IList<NameNode> Parameters { get { return this.parameters; } }

        public INode BodyNode { get { return this.body; } }
    }
}
