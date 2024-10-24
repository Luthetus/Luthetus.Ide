using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when invoking a function.
/// </summary>
public sealed class FunctionParameterEntryNode : ISyntaxNode
{
    public FunctionParameterEntryNode(
        IExpressionNode expressionNode,
        bool hasOutKeyword,
        bool hasInKeyword,
        bool hasRefKeyword)
    {
        ExpressionNode = expressionNode;
        HasOutKeyword = hasOutKeyword;
        HasInKeyword = hasInKeyword;
        HasRefKeyword = hasRefKeyword;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public IExpressionNode ExpressionNode { get; }
    public bool HasOutKeyword { get; }
    public bool HasInKeyword { get; }
    public bool HasRefKeyword { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionParameterEntryNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		_childListIsDirty;
    	
    	var childCount = 1; // ExpressionNode
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = ExpressionNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
