using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record GenericArgumentEntryNodeTests
{
    public GenericArgumentEntryNode(TypeClauseNode typeClauseNode)
    {
        TypeClauseNode = typeClauseNode;

        var children = new List<ISyntax>
        {
            TypeClauseNode
        };

        ChildBag = children.ToImmutableArray();
    }

    public TypeClauseNode TypeClauseNode { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.GenericArgumentEntryNode;
}