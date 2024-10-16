using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ConstructorInvocationExpressionNode : IExpressionNode
{
    public ConstructorInvocationExpressionNode(
        KeywordToken newKeywordToken,
        TypeClauseNode typeClauseNode,
        FunctionParametersListingNode? functionParametersListingNode,
        ObjectInitializationParametersListingNode? objectInitializationParametersListingNode)
    {
        NewKeywordToken = newKeywordToken;
        ResultTypeClauseNode = typeClauseNode;
        FunctionParametersListingNode = functionParametersListingNode;
        ObjectInitializationParametersListingNode = objectInitializationParametersListingNode;

        SetChildList();
    }

    public KeywordToken NewKeywordToken { get; }
    public TypeClauseNode ResultTypeClauseNode { get; }
    public FunctionParametersListingNode? FunctionParametersListingNode { get; }
    public ObjectInitializationParametersListingNode? ObjectInitializationParametersListingNode { get; }
    
    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ConstructorInvocationExpressionNode;
    
    public void SetChildList()
    {
    	var childCount = 2; // NewKeywordToken, ResultTypeClauseNode,
    	if (FunctionParametersListingNode is not null)
            childCount++;
        if (ObjectInitializationParametersListingNode is not null)
            childCount++;
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = NewKeywordToken;
		childList[i++] = ResultTypeClauseNode;
		if (FunctionParametersListingNode is not null)
            childList[i++] = FunctionParametersListingNode;
        if (ObjectInitializationParametersListingNode is not null)
            childList[i++] = ObjectInitializationParametersListingNode;
            
        ChildList = childList;
    }
}
