using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// I'm not sure how to go about property initialization of an object versus collection initialization.
/// For now I'll rather hackily set the <see cref="ExpressionNode"/> only if a comma immediately follows it.
///
/// Then to know if an instance of this type represents collection initialization
/// one can check <see cref="IsCollectionInitialization"/> to know whether
/// the token was 'default' or not.
/// </summary>
public sealed class ObjectInitializationParameterEntryNode : ISyntaxNode
{
    public ObjectInitializationParameterEntryNode(
        IdentifierToken propertyIdentifierToken,
        EqualsToken equalsToken,
        IExpressionNode expressionNode)
    {
        PropertyIdentifierToken = propertyIdentifierToken;
        EqualsToken = equalsToken;
        ExpressionNode = expressionNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public IdentifierToken PropertyIdentifierToken { get; set; }
    public EqualsToken EqualsToken { get; set; }
    public IExpressionNode ExpressionNode { get; set; }
    public bool IsCollectionInitialization => !PropertyIdentifierToken.ConstructorWasInvoked;

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ObjectInitializationParameterEntryNode;
    
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
    	
    	var childCount = 2; // PropertyIdentifierToken, ExpressionNode,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = PropertyIdentifierToken;
		childList[i++] = ExpressionNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}