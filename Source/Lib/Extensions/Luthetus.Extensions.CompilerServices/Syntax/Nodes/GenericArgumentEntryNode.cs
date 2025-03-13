namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when defining a syntax which contains a generic type.
/// </summary>
public sealed class GenericArgumentEntryNode : ISyntaxNode
{
	public GenericArgumentEntryNode(TypeClauseNode typeClauseNode)
	{
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.GenericArgumentEntryNode++;
	
		TypeClauseNode = typeClauseNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public TypeClauseNode TypeClauseNode { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.GenericArgumentEntryNode;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 1; // TypeClauseNode,

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = TypeClauseNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}