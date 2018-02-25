namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;

    public class FuncNode : IStatementNode
    {
        private string name;
        private IList<NameNode> parameters;
        private IStatementNode body;
        private TypeInfo returnType;

        public FuncNode(string name, IList<NameNode> parameters, IStatementNode body, TypeInfo returnType)
        {
            this.name = name;
            this.parameters = parameters;
            this.body = body;
            this.returnType = returnType;
        }

        public string Name { get { return this.name; } }

        public IList<NameNode> Parameters { get { return this.parameters; } }

        public IStatementNode BodyNode { get { return this.body; } }

        public TypeInfo ReturnType { get { return this.returnType; } }
    }
}

