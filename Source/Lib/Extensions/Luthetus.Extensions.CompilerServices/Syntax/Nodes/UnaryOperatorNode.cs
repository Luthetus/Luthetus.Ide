namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class UnaryOperatorNode : ISyntaxNode
{
	public UnaryOperatorNode(
		TypeReference operandTypeReference,
		SyntaxToken operatorToken,
		TypeReference resultTypeReference)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.UnaryOperatorNode++;
		#endif
	
		OperandTypeReference = operandTypeReference;
		OperatorToken = operatorToken;
		ResultTypeReference = resultTypeReference;
	}

	public TypeReference OperandTypeReference { get; }
	public SyntaxToken OperatorToken { get; }
	public TypeReference ResultTypeReference { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.UnaryOperatorNode;

	#if DEBUG	
	~UnaryOperatorNode()
	{
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.UnaryOperatorNode--;
	}
	#endif
}