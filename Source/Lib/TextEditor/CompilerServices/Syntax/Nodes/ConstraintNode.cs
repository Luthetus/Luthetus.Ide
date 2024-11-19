using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Examples:<br/>
/// 
/// public T Clone&lt;T&gt;(T item) where T : class<br/>
/// {<br/>
/// &#9;return item;<br/>
/// }<br/>
/// 
/// public T Clone&lt;T&gt;(T item) where T : class => item;<br/>
/// </summary>
public sealed class ConstraintNode : ISyntaxNode
{
    public ConstraintNode(ImmutableArray<ISyntaxToken> innerTokens)
    {
        InnerTokens = innerTokens;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    /// <summary>
    /// TODO: For now, just grab all tokens and put them in an array...
    /// ...In the future parse the tokens. (2023-10-19)
    /// </summary>
    public ImmutableArray<ISyntaxToken> InnerTokens { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ConstraintNode;
    
    public int GetStartInclusiveIndex()
    {
    	return 0;
    }
    
    public int GetEndExclusiveIndex()
    {
    	return 0;
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	_childList = InnerTokens.ToArray();
        
    	_childListIsDirty = false;
    	return _childList;
    }
}