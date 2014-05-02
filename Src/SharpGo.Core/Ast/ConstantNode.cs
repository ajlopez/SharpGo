namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ConstantNode : INode
    {
        private object value;

        public ConstantNode(object value)
        {
            this.value = value;
        }

        public object Value { get { return this.value; } }
    }
}
