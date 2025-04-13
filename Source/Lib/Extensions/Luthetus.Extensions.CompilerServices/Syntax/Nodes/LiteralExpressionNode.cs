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

	// private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken LiteralSyntaxToken { get; }
	public TypeReference ResultTypeReference { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.LiteralExpressionNode;

	/*public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 1; // LiteralSyntaxToken,

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = LiteralSyntaxToken;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}*/
}
