namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class FuncNode : INode
    {
        private string name;
        private INode body;

        public FuncNode(string name, INode body)
        {
            this.name = name;
            this.body = body;
        }

        public string Name { get { return this.name; } }

        public INode BodyNode { get { return this.body; } }
    }
}
