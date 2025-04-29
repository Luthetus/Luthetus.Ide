using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class ConstructorInvocationExpressionNode : IInvocationNode
{
	/// <summary>
	/// The <see cref="GenericParametersListingNode"/> is located
	/// on the <see cref="TypeClauseNode"/>.
	/// </summary>
	public ConstructorInvocationExpressionNode(
		SyntaxToken newKeywordToken,
		TypeReference typeReference,
		FunctionParameterListing functionParameterListing)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ConstructorInvocationExpressionNode++;
		#endif
	
		NewKeywordToken = newKeywordToken;
		ResultTypeReference = typeReference;
		FunctionParameterListing = functionParameterListing;
	}

	public SyntaxToken NewKeywordToken { get; }
	public TypeReference ResultTypeReference { get; set; }
	public FunctionParameterListing FunctionParameterListing { get; set; }

	public ConstructorInvocationStageKind ConstructorInvocationStageKind { get; set; } = ConstructorInvocationStageKind.Unset;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ConstructorInvocationExpressionNode;
	
	public bool IsParsingFunctionParameters { get; set; }

	#if DEBUG	
	~ConstructorInvocationExpressionNode()
	{
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ConstructorInvocationExpressionNode--;
	}
	#endif
}
