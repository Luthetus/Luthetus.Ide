using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class WithExpressionNode : IExpressionNode
{
	public WithExpressionNode(VariableReference variableReference)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.WithExpressionNode++;
		#endif
	
		VariableReference = variableReference;
		ResultTypeReference = variableReference.ResultTypeReference;
	}

	public VariableReference VariableReference { get; }
	public TypeReference ResultTypeReference { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.WithExpressionNode;
}
