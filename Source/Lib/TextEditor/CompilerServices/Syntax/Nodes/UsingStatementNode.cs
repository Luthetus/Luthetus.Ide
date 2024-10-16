using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class UsingStatementNode : ISyntaxNode
{
    public UsingStatementNode(KeywordToken keywordToken, IdentifierToken namespaceIdentifier)
    {
        KeywordToken = keywordToken;
        NamespaceIdentifier = namespaceIdentifier;

        SetChildList();
    }

    public KeywordToken KeywordToken { get; }
    public IdentifierToken NamespaceIdentifier { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.UsingStatementNode;
    
    public void SetChildList()
    {
    	ChildList = new ISyntax[]
        {
            KeywordToken,
            NamespaceIdentifier
        };
    }
}