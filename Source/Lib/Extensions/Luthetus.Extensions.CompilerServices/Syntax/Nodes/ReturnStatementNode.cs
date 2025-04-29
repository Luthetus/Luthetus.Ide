using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class ReturnStatementNode : IExpressionNode
{
	public ReturnStatementNode(SyntaxToken keywordToken, IExpressionNode expressionNode)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ReturnStatementNode++;
		#endif
	
		KeywordToken = keywordToken;
		ExpressionNode = expressionNode;
	}

	public SyntaxToken KeywordToken { get; }
	public IExpressionNode ExpressionNode { get; set; }
	public TypeReference ResultTypeReference => ExpressionNode.ResultTypeReference;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ReturnStatementNode;

	#if DEBUG	
	~ReturnStatementNode()
	{
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ReturnStatementNode--;
	}
	#endif
}