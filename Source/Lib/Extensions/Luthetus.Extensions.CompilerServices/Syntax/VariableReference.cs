using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax;

public record struct VariableReference
{
	public static VariableReference Empty { get; } = default;

	public VariableReference(
		SyntaxToken variableIdentifierToken,
		VariableDeclarationNode variableDeclarationNode,
		TypeReference resultTypeReference,
		bool isFabricated)
	{
		VariableIdentifierToken = variableIdentifierToken;
		VariableDeclarationNode = variableDeclarationNode;
		ResultTypeReference = resultTypeReference;
		IsFabricated = isFabricated;
	}
	
	public VariableReference(VariableReferenceNode variableReferenceNode)
	{
		VariableIdentifierToken = variableReferenceNode.VariableIdentifierToken;
		VariableDeclarationNode = variableReferenceNode.VariableDeclarationNode;
		ResultTypeReference = variableReferenceNode.ResultTypeReference;
		IsFabricated = variableReferenceNode.IsFabricated;
	}

	public SyntaxToken VariableIdentifierToken { get; }
	public VariableDeclarationNode VariableDeclarationNode { get; }
	public TypeReference ResultTypeReference { get; }
	public bool IsFabricated { get; }
}
