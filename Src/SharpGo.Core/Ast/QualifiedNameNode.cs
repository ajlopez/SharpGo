namespace SharpGo.Core.Ast
{
    using System;
    using SharpGo.Core.Language;

    public class QualifiedNameNode : IExpressionNode
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

        public TypeInfo TypeInfo
        {
            get { throw new NotImplementedException(); }
        }
    }
}
