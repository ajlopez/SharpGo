namespace SharpGo.Core.Ast
{
    using SharpGo.Core.Language.TypeInfos;

    public interface IExpressionNode : INode
    {
        TypeInfo TypeInfo { get; }
    }
}
