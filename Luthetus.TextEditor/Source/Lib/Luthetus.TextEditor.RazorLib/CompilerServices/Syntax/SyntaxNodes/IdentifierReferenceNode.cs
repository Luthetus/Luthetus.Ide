using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

public sealed record IdentifierReferenceNode : IExpressionNode
{
    public IdentifierReferenceNode(IdentifierToken identifierToken, TypeClauseNode typeClauseNode)
    {
        IdentifierToken = identifierToken;
        TypeClauseNode = typeClauseNode;

        ChildBag = new ISyntax[]
        {
            IdentifierToken,
            TypeClauseNode,
        }.ToImmutableArray();
    }

    public IdentifierToken IdentifierToken { get; }
    public TypeClauseNode TypeClauseNode { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.IdentifierReferenceNode;
}