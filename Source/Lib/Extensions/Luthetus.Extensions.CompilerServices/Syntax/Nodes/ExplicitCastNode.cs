using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class ExplicitCastNode : IExpressionNode
{
	public ExplicitCastNode(
		SyntaxToken openParenthesisToken,
		TypeReference resultTypeReference,
		SyntaxToken closeParenthesisToken)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ExplicitCastNode++;
		#endif
	
		OpenParenthesisToken = openParenthesisToken;
		ResultTypeReference = resultTypeReference;
		CloseParenthesisToken = closeParenthesisToken;
	}

	public ExplicitCastNode(SyntaxToken openParenthesisToken, TypeReference resultTypeReference)
		: this(openParenthesisToken, resultTypeReference, default)
	{
	}

	public SyntaxToken OpenParenthesisToken { get; }
	public TypeReference ResultTypeReference { get; }
	public SyntaxToken CloseParenthesisToken { get; private set; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ExplicitCastNode;

	public ExplicitCastNode SetCloseParenthesisToken(SyntaxToken closeParenthesisToken)
	{
		CloseParenthesisToken = closeParenthesisToken;
		return this;
	}
}
