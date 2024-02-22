using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Expression;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when invoking a function.
/// </summary>
public sealed record FunctionParameterEntryNode : ISyntaxNode
{
    public FunctionParameterEntryNode(
        IExpressionNode expressionNode,
        bool hasOutKeyword,
        bool hasInKeyword,
        bool hasRefKeyword)
    {
        ExpressionNode = expressionNode;
        HasOutKeyword = hasOutKeyword;
        HasInKeyword = hasInKeyword;
        HasRefKeyword = hasRefKeyword;

        var children = new List<ISyntax>
        {
            ExpressionNode
        };

        ChildList = children.ToImmutableArray();
    }

    public IExpressionNode ExpressionNode { get; }
    public bool HasOutKeyword { get; }
    public bool HasInKeyword { get; }
    public bool HasRefKeyword { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionParameterEntryNode;
}
