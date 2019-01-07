namespace SharpGo.Core.Ast
{
    using System.Collections.Generic;
    using SharpGo.Core.Language.TypeInfos;

    public class FuncNode : IStatementNode
    {
        private readonly string name;
        private readonly IList<NameNode> parameters;
        private readonly IStatementNode body;
        private readonly TypeInfo returnType;

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

