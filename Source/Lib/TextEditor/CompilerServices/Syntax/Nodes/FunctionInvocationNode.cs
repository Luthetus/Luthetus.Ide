using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class FunctionInvocationNode : IExpressionNode
{
    public FunctionInvocationNode(
        IdentifierToken functionInvocationIdentifierToken,
        FunctionDefinitionNode? functionDefinitionNode,
        GenericParametersListingNode? genericParametersListingNode,
        FunctionParametersListingNode functionParametersListingNode,
        TypeClauseNode resultTypeClauseNode)
    {
        FunctionInvocationIdentifierToken = functionInvocationIdentifierToken;
        FunctionDefinitionNode = functionDefinitionNode;
        GenericParametersListingNode = genericParametersListingNode;
        FunctionParametersListingNode = functionParametersListingNode;
        ResultTypeClauseNode = resultTypeClauseNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public IdentifierToken FunctionInvocationIdentifierToken { get; }
    public FunctionDefinitionNode? FunctionDefinitionNode { get; }
    public GenericParametersListingNode? GenericParametersListingNode { get; }
    public FunctionParametersListingNode FunctionParametersListingNode { get; }
    public TypeClauseNode ResultTypeClauseNode { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionInvocationNode;

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
    	
    	var childCount = 3; // FunctionInvocationIdentifierToken, ...FunctionParametersListingNode, ResultTypeClauseNode,
        if (FunctionDefinitionNode is not null)
            childCount++;
        if (GenericParametersListingNode is not null)
            childCount++;
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = FunctionInvocationIdentifierToken;
		if (FunctionDefinitionNode is not null)
            childList[i++] = FunctionDefinitionNode;
        if (GenericParametersListingNode is not null)
            childList[i++] = GenericParametersListingNode;
		childList[i++] = FunctionParametersListingNode;
		childList[i++] = ResultTypeClauseNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}