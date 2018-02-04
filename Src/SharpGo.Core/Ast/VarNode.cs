namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;

    public class VarNode : IStatementNode
    {
        private string name;
        private TypeInfo typeinfo;
        private IExpressionNode expression;

        public VarNode(string name, TypeInfo typeinfo, IExpressionNode expression)
        {
            this.name = name;
            this.typeinfo = typeinfo;
            this.expression = expression;

            if (typeinfo == null)
                this.typeinfo = this.expression.TypeInfo;
        }

        public string Name { get { return this.name; } }

        public IExpressionNode ExpressionNode { get { return this.expression; } }

        public TypeInfo TypeInfo { get { return this.typeinfo; } }
    }
}

