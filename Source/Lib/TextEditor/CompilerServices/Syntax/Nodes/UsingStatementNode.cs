namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class UsingStatementNode : ISyntaxNode
{
    public UsingStatementNode(SyntaxToken keywordToken, SyntaxToken namespaceIdentifier)
    {
        KeywordToken = keywordToken;
        NamespaceIdentifier = namespaceIdentifier;
    }

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public SyntaxToken KeywordToken { get; }
    public SyntaxToken NamespaceIdentifier { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.UsingStatementNode;
    
    public IReadOnlyList<ISyntax> GetChildList()
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