using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

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
        
        SetChildList();
    }

    public IdentifierToken FunctionInvocationIdentifierToken { get; }
    public FunctionDefinitionNode? FunctionDefinitionNode { get; }
    public GenericParametersListingNode? GenericParametersListingNode { get; }
    public FunctionParametersListingNode FunctionParametersListingNode { get; }
    public TypeClauseNode ResultTypeClauseNode { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionInvocationNode;

    public void SetChildList()
    {
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
            
        ChildList = childList;
    }
}