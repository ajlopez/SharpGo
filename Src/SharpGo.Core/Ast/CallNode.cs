namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;

    public class CallNode : IExpressionNode
    {
        private string name;
        private IList<IExpressionNode> arguments;

        public CallNode(string name, IList<IExpressionNode> arguments)
        {
            this.name = name;
            this.arguments = arguments;
        }

        public string Name { get { return this.name; } }

        public IList<IExpressionNode> Arguments { get { return this.arguments; } }

        public TypeInfo TypeInfo
        {
            get { throw new NotImplementedException(); }
        }
    }
}
