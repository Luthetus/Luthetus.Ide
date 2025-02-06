using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>TODO: Correctly implement this node. For now, just skip over it when parsing.</summary>
[Obsolete($"Use: {nameof(ConstructorInvocationExpressionNode)}.{nameof(ConstructorInvocationExpressionNode.ObjectInitializationParametersListingNode)}")]
public sealed class ObjectInitializationNode : ISyntaxNode
{
    public ObjectInitializationNode(OpenBraceToken openBraceToken, CloseBraceToken closeBraceToken)
    {
        OpenBraceToken = openBraceToken;
        CloseBraceToken = closeBraceToken;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public OpenBraceToken OpenBraceToken { get; }
    public CloseBraceToken CloseBraceToken { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ObjectInitializationNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 2; // OpenBraceToken, CloseBraceToken,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenBraceToken;
		childList[i++] = CloseBraceToken;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}