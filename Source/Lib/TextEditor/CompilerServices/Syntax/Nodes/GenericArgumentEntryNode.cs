using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when defining a syntax which contains a generic type.
/// </summary>
public sealed class GenericArgumentEntryNode : ISyntaxNode
{
    public GenericArgumentEntryNode(TypeClauseNode typeClauseNode)
    {
        TypeClauseNode = typeClauseNode;

        var children = new List<ISyntax>
        {
            TypeClauseNode
        };

        ChildList = children.ToImmutableArray();
    }

    public TypeClauseNode TypeClauseNode { get; }

    public ImmutableArray<ISyntax> ChildList { get; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.GenericArgumentEntryNode;
}