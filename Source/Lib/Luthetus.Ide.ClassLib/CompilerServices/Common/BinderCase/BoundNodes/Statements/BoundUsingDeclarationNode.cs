using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundUsingStatementNode : ISyntaxNode
{
    public BoundUsingStatementNode(
        KeywordToken keywordToken,
        IdentifierToken namespaceIdentifier)
    {
        KeywordToken = keywordToken;
        NamespaceIdentifier = namespaceIdentifier;

        Children = new ISyntax[]
        {
            KeywordToken,
            NamespaceIdentifier
        }.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; init; }
    public IdentifierToken NamespaceIdentifier { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundUsingStatementNode;
}
