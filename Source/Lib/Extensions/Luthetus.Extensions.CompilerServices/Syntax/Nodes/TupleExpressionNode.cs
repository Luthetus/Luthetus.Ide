using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class TupleExpressionNode : IExpressionNode
{
	public TupleExpressionNode()
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.TupleExpressionNode++;
		#endif
	}

	public TypeReference ResultTypeReference { get; } = TypeFacts.Empty.ToTypeReference();

	public List<IExpressionNode> InnerExpressionList { get; } = new();

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.TupleExpressionNode;

	#if DEBUG	
	~TupleExpressionNode()
	{
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.TupleExpressionNode--;
	}
	#endif
}
