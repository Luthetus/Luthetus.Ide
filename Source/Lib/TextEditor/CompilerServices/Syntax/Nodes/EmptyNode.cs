using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// At times, this node is used in place of 'null'.
/// </summary>
public sealed record EmptyNode : ISyntaxNode
{
    public EmptyNode()
    {
    }

    public ImmutableArray<ISyntax> ChildList { get; } = ImmutableArray<ISyntax>.Empty;

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.EmptyNode;
}
