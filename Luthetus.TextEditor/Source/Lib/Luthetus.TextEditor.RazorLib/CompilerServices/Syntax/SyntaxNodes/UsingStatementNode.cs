using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

public sealed record UsingStatementNode : ISyntaxNode
{
    public UsingStatementNode(KeywordToken keywordToken, IdentifierToken namespaceIdentifier)
    {
        KeywordToken = keywordToken;
        NamespaceIdentifier = namespaceIdentifier;

        ChildBag = new ISyntax[]
        {
            KeywordToken,
            NamespaceIdentifier
        }.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; }
    public IdentifierToken NamespaceIdentifier { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.UsingStatementNode;
}