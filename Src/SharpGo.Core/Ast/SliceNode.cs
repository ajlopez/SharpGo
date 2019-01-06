namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;
    using SharpGo.Core.Language.TypeInfos;

    public class SliceNode : IExpressionNode
    {
        private string name;
        private IExpressionNode low;
        private IExpressionNode high;

        public SliceNode(string name, IExpressionNode low, IExpressionNode high)
        {
            this.name = name;
            this.low = low;
            this.high = high;
        }

        public string Name { get { return this.name; } }

        public IExpressionNode Low { get { return this.low; } }

        public IExpressionNode High { get { return this.high; } }

        public TypeInfo TypeInfo
        {
            get { throw new NotImplementedException(); }
        }
    }
}
