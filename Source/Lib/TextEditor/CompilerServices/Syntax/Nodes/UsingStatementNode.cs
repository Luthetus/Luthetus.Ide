using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class UsingStatementNode : ISyntaxNode
{
    public UsingStatementNode(KeywordToken keywordToken, INameToken nameToken)
    {
        KeywordToken = keywordToken;
        NameToken = nameToken;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public KeywordToken KeywordToken { get; }
    public INameToken NameToken { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.UsingStatementNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	_childList = new ISyntax[]
        {
            KeywordToken,
            NameToken
        };
        
    	_childListIsDirty = false;
    	return _childList;
    }
}