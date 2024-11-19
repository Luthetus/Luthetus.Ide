using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class UsingStatementNode : ISyntaxNode
{
    public UsingStatementNode(KeywordToken keywordToken, IdentifierToken namespaceIdentifier)
    {
        KeywordToken = keywordToken;
        NamespaceIdentifier = namespaceIdentifier;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public KeywordToken KeywordToken { get; }
    public IdentifierToken NamespaceIdentifier { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.UsingStatementNode;
    
    public int GetStartInclusiveIndex()
    {
    }
    
    public int GetEndExclusiveIndex()
    {
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	_childList = new ISyntax[]
        {
            KeywordToken,
            NamespaceIdentifier
        };
        
    	_childListIsDirty = false;
    	return _childList;
    }
}