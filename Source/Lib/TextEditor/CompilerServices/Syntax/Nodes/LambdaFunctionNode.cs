using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// TODO: Starting implementation of lambda function parsing...
/// 	  ...This node doesn't exist though, right?
///       It is just a variable declaration node,
///       and an extra step of declaring a hidden-static function
///       and providing the lambda's codeblock as the function's codeblock,
///       (and arguments, return type, etc...).
/// </summary>
public sealed class LambdaFunctionNode : IExpressionNode
{
    public LambdaFunctionNode(
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