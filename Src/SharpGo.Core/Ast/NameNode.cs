namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;

    public class NameNode : IExpressionNode
    {
        private string name;

        public NameNode(string name)
        {
            this.name = name;
        }

        public string Name { get { return this.name; } }

        public TypeInfo TypeInfo
        {
            get { throw new NotImplementedException(); }
        }
    }
}
