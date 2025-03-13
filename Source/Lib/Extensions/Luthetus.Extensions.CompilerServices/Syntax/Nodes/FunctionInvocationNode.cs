using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class FunctionInvocationNode : IExpressionNode
{
	public FunctionInvocationNode(
		SyntaxToken functionInvocationIdentifierToken,
		FunctionDefinitionNode? functionDefinitionNode,
		GenericParametersListingNode? genericParametersListingNode,
		FunctionParametersListingNode functionParametersListingNode,
		TypeClauseNode resultTypeClauseNode)
	{
		// Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.FunctionInvocationNode++;
	
		FunctionInvocationIdentifierToken = functionInvocationIdentifierToken;
		FunctionDefinitionNode = functionDefinitionNode;
		GenericParametersListingNode = genericParametersListingNode;
		FunctionParametersListingNode = functionParametersListingNode;
		ResultTypeClauseNode = resultTypeClauseNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken FunctionInvocationIdentifierToken { get; }
	public FunctionDefinitionNode? FunctionDefinitionNode { get; }
	public GenericParametersListingNode? GenericParametersListingNode { get; private set; }
	public FunctionParametersListingNode FunctionParametersListingNode { get; private set; }
	public TypeClauseNode ResultTypeClauseNode { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.FunctionInvocationNode;

	public FunctionInvocationNode SetGenericParametersListingNode(GenericParametersListingNode genericParametersListingNode)
	{
		GenericParametersListingNode = genericParametersListingNode;
		return this;
	}

	public FunctionInvocationNode SetFunctionParametersListingNode(FunctionParametersListingNode functionParametersListingNode)
	{
		FunctionParametersListingNode = functionParametersListingNode;
		return this;
	}

	public IReadOnlyList<ISyntax> GetChildList()
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