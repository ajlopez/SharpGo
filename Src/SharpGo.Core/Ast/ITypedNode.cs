namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SharpGo.Core.Language;

    public interface ITypedNode : INode
    {
        TypeInfo TypeInfo { get; }
    }
}
