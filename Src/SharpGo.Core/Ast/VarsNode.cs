namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;
    using SharpGo.Core.Language.TypeInfos;

    public class VarsNode : IStatementNode
    {
        private IList<string> names;
        private TypeInfo typeinfo;
        private IList<IExpressionNode> expressions;

        public VarsNode(IList<string> names, TypeInfo typeinfo, IList<IExpressionNode> expressions)
        {
            this.names = names;
            this.typeinfo = typeinfo;
            this.expressions = expressions;
        }

        public IList<string> Names { get { return this.names; } }

        public IList<IExpressionNode> ExpressionNodes { get { return this.expressions; } }

        public TypeInfo TypeInfo { get { return this.typeinfo; } }
    }
}

