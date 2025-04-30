using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class LiteralExpressionNode : IExpressionNode
{
	public LiteralExpressionNode(SyntaxToken literalSyntaxToken, TypeReference typeReference)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.LiteralExpressionNode++;
		#endif
	
		LiteralSyntaxToken = literalSyntaxToken;
		ResultTypeReference = typeReference;
	}

	public SyntaxToken LiteralSyntaxToken { get; }
	public TypeReference ResultTypeReference { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.LiteralExpressionNode;

	#if DEBUG	
	~LiteralExpressionNode()
	{
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.LiteralExpressionNode--;
	}
	#endif
}
