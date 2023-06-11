using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundNamespaceStatementNode : ISyntaxNode
{
    public BoundNamespaceStatementNode(
        KeywordToken keywordToken,
        IdentifierToken identifierToken,
        ImmutableArray<BoundNamespaceEntryNode> children)
    {
        KeywordToken = keywordToken;
        IdentifierToken = identifierToken;

        Children = children
            .Select(x => (ISyntax)x)
            .ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; init; }
    public IdentifierToken IdentifierToken { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundNamespaceStatementNode;
}