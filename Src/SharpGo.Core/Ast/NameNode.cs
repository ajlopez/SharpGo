namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class NameNode : INode
    {
        private string name;

        public NameNode(string name)
        {
            this.name = name;
        }

        public string Name { get { return this.name; } }
    }
}
