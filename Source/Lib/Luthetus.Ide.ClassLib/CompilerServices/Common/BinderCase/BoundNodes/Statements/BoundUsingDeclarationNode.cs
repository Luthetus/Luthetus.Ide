using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public class BoundUsingDeclarationNode : ISyntaxNode
{
    public BoundUsingDeclarationNode(
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

    public KeywordToken KeywordToken { get; }
    public IdentifierToken NamespaceIdentifier { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundUsingDeclarationNode;
}
