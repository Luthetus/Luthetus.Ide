using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class FunctionInvocationNode : IInvocationNode, IGenericParameterNode
{
	public FunctionInvocationNode(
		SyntaxToken functionInvocationIdentifierToken,
		FunctionDefinitionNode? functionDefinitionNode,
		GenericParameterListing genericParameterListing,
		FunctionParameterListing functionParameterListing,
		TypeReference resultTypeReference)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.FunctionInvocationNode++;
		#endif
	
		FunctionInvocationIdentifierToken = functionInvocationIdentifierToken;
		FunctionDefinitionNode = functionDefinitionNode;
		GenericParameterListing = genericParameterListing;
		FunctionParameterListing = functionParameterListing;
		ResultTypeReference = resultTypeReference;
	}

	public SyntaxToken FunctionInvocationIdentifierToken { get; }
	public FunctionDefinitionNode? FunctionDefinitionNode { get; }
	public GenericParameterListing GenericParameterListing { get; set; }
	public FunctionParameterListing FunctionParameterListing { get; private set; }
	public TypeReference ResultTypeReference { get; private set; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.FunctionInvocationNode;
	
	public bool IsParsingFunctionParameters { get; set; }
	public bool IsParsingGenericParameters { get; set; }

	public void SetGenericParameterListing(GenericParameterListing genericParameterListing)
	{
		GenericParameterListing = genericParameterListing;
	}
	
	public void SetGenericParameterListingCloseAngleBracketToken(SyntaxToken closeAngleBracketToken)
	{
		GenericParameterListing.SetCloseAngleBracketToken(closeAngleBracketToken);
	}

	public void SetFunctionParameterListing(FunctionParameterListing functionParameterListing)
	{
		FunctionParameterListing = functionParameterListing;
	}
	
	public void SetFunctionParameterListingCloseParenthesisToken(SyntaxToken closeParenthesisToken)
	{
		FunctionParameterListing.SetCloseParenthesisToken(closeParenthesisToken);
	}
	
	/// <summary>
	/// TODO: 'BindFunctionInvocationNode' takes the instance of the 'FunctionInvocationNode',
	/// but in order to know the result type clause node we need to invoke
	/// 'BindFunctionInvocationNode', this is odd?
	/// </summary>
	public void SetResultTypeReference(TypeReference typeReference)
	{
		ResultTypeReference = typeReference;
	}
}