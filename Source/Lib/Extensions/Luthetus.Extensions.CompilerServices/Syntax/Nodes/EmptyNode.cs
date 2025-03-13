namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// At times, this node is used in place of 'null'.
/// </summary>
public sealed class EmptyNode : ISyntaxNode
{
	public EmptyNode()
	{
		// Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.EmptyNode++;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.EmptyNode;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		_childList = Array.Empty<ISyntax>();

		_childListIsDirty = false;
		return _childList;
	}
}
