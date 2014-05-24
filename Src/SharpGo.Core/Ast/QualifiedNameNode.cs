namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class QualifiedNameNode : INode
    {
        private string packagename;
        private string name;

        public QualifiedNameNode(string packagename, string name)
        {
            this.packagename = packagename;
            this.name = name;
        }

        public string PackageName { get { return this.packagename; } }

        public string Name { get { return this.name; } }
    }
}
