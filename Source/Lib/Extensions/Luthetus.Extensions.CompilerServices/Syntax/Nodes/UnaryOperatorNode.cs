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

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public TypeReference OperandTypeReference { get; }
	public SyntaxToken OperatorToken { get; }
	public TypeReference ResultTypeReference { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.UnaryOperatorNode;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		_childList = new ISyntax[]
		{
			OperatorToken,
		};

		_childListIsDirty = false;
		return _childList;
	}
}