using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record UsingStatementNode : ISyntaxNode
{
    public UsingStatementNode(KeywordToken keywordToken, IdentifierToken namespaceIdentifier)
    {
        KeywordToken = keywordToken;
        NamespaceIdentifier = namespaceIdentifier;

        ChildList = new ISyntax[]
        {
            KeywordToken,
            NamespaceIdentifier
        }.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; }
    public IdentifierToken NamespaceIdentifier { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.UsingStatementNode;
}