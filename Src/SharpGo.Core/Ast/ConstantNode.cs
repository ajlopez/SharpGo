namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;

    public class ConstantNode : IExpressionNode
    {
        private TypeInfo typeinfo;
        private object value;

        public ConstantNode(object value)
        {
            this.value = value;
            this.typeinfo = TypeInfo.GetTypeInfo(value);
        }

        public object Value { get { return this.value; } }

        public TypeInfo TypeInfo { get { return this.typeinfo; } }
    }
}
