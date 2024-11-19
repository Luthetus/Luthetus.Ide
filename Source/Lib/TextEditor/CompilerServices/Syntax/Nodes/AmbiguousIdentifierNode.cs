using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Example usage: One finds a <see cref="IdentifierToken"/>, but must
/// continue parsing in order to know if it is a reference to a
/// function, type, variable, or etc...
///
/// TODO: Permit this type to have a nullable GenericParametersListingNode
/// </summary>
public sealed class AmbiguousIdentifierNode : ISyntaxNode
{
    public AmbiguousIdentifierNode(IdentifierToken identifierToken)
    {
        IdentifierToken = identifierToken;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public IdentifierToken IdentifierToken { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.AmbiguousIdentifierNode;
    
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
            IdentifierToken,
        };
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
