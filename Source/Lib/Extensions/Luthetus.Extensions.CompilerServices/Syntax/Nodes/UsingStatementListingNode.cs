namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class UsingStatementListingNode : ISyntaxNode
{
	public UsingStatementListingNode()
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.UsingStatementListingNode++;
		#endif
	}

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
	}
}