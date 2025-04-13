namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class UsingStatementListingNode : ISyntaxNode
{
	public UsingStatementListingNode()
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.UsingStatementListingNode++;
		#endif
	}

	// private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	/// <summary>
	/// Note: don't store as ISyntax in order to avoid boxing.
	/// If someone explicitly invokes 'GetChildList()' then box at that point
	/// but 'GetChildList()' is far less likely to be invoked for this type.
	/// </summary>
	private List<(SyntaxToken KeywordToken, SyntaxToken NamespaceIdentifier)> _usingStatementTupleList { get; set; } = new();

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.UsingStatementListingNode;
	
	public void AddUsingStatementTuple((SyntaxToken KeywordToken, SyntaxToken NamespaceIdentifier) usingStatementTuple)
	{
		_usingStatementTupleList.Add(usingStatementTuple);
		_childListIsDirty = true;
	}

	/*public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;
			
		_childList = _usingStatementTupleList.Select(x => (ISyntax)x.NamespaceIdentifier).ToArray();

		_childListIsDirty = false;
		return _childList;
	}*/
}