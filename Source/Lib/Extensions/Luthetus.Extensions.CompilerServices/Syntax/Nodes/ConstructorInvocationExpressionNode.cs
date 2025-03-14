using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class ConstructorInvocationExpressionNode : IExpressionNode
{
	/// <summary>
	/// The <see cref="GenericParametersListingNode"/> is located
	/// on the <see cref="TypeClauseNode"/>.
	/// </summary>
	public ConstructorInvocationExpressionNode(
		SyntaxToken newKeywordToken,
		TypeClauseNode typeClauseNode,
		FunctionParametersListingNode? functionParametersListingNode,
		ObjectInitializationParametersListingNode? objectInitializationParametersListingNode)
	{
		// Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ConstructorInvocationExpressionNode++;
	
		NewKeywordToken = newKeywordToken;
		ResultTypeClauseNode = typeClauseNode;
		FunctionParametersListingNode = functionParametersListingNode;
		ObjectInitializationParametersListingNode = objectInitializationParametersListingNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken NewKeywordToken { get; }
	public TypeClauseNode ResultTypeClauseNode { get; private set; }
	public FunctionParametersListingNode? FunctionParametersListingNode { get; private set; }
	public ObjectInitializationParametersListingNode? ObjectInitializationParametersListingNode { get; private set; }

	public ConstructorInvocationStageKind ConstructorInvocationStageKind { get; set; } = ConstructorInvocationStageKind.Unset;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ConstructorInvocationExpressionNode;

	public ConstructorInvocationExpressionNode SetTypeClauseNode(TypeClauseNode? resultTypeClauseNode)
	{
		ResultTypeClauseNode = resultTypeClauseNode;

		_childListIsDirty = true;
		return this;
	}

	public ConstructorInvocationExpressionNode SetFunctionParametersListingNode(FunctionParametersListingNode? functionParametersListingNode)
	{
		FunctionParametersListingNode = functionParametersListingNode;

		_childListIsDirty = true;
		return this;
	}

	public ConstructorInvocationExpressionNode SetObjectInitializationParametersListingNode(ObjectInitializationParametersListingNode? objectInitializationParametersListingNode)
	{
		ObjectInitializationParametersListingNode = objectInitializationParametersListingNode;

		_childListIsDirty = true;
		return this;
	}

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

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

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}
