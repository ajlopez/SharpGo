namespace SharpGo.Core.Ast
{
    using System;
    using SharpGo.Core.Language.TypeInfos;

    public class SliceNode : IExpressionNode
    {
        private readonly string name;
        private readonly IExpressionNode low;
        private readonly IExpressionNode high;

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
