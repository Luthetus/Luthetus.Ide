using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class InheritanceStatementNode : ISyntaxNode
{
    public InheritanceStatementNode(TypeClauseNode parentTypeClauseNode)
    {
        ParentTypeClauseNode = parentTypeClauseNode;

        ChildList = new ISyntax[]
        {
            ParentTypeClauseNode
        }.ToImmutableArray();
    }

    public TypeClauseNode ParentTypeClauseNode { get; }

    public ImmutableArray<ISyntax> ChildList { get; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.InheritanceStatementNode;
}