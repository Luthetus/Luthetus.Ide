using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// At times, this node is used in place of 'null'.
/// </summary>
public sealed class EmptyNode : ISyntaxNode
{
    public EmptyNode()
    {
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.EmptyNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	_childList = Array.Empty<ISyntax>();
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
