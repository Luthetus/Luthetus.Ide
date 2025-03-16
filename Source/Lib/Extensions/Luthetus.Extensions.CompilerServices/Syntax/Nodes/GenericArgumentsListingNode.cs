namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when defining a syntax which contains a generic type.
/// </summary>
public sealed class GenericArgumentsListingNode : ISyntaxNode
{
	public static readonly List<GenericArgumentEntryNode> __empty = new();

	public GenericArgumentsListingNode(
		SyntaxToken openAngleBracketToken,
		List<GenericArgumentEntryNode> genericArgumentEntryNodeList,
		SyntaxToken closeAngleBracketToken)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.GenericArgumentsListingNode++;
		#endif
	
		OpenAngleBracketToken = openAngleBracketToken;
		GenericArgumentEntryNodeList = genericArgumentEntryNodeList;
		CloseAngleBracketToken = closeAngleBracketToken;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken OpenAngleBracketToken { get; }
	public List<GenericArgumentEntryNode> GenericArgumentEntryNodeList { get; }
	public SyntaxToken CloseAngleBracketToken { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.GenericArgumentsListingNode;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		// OpenAngleBracketToken, GenericArgumentEntryNodeList.Length, CloseAngleBracketToken
		var childCount =
			1 +                                   // OpenAngleBracketToken,
			GenericArgumentEntryNodeList.Count + // GenericArgumentEntryNodeList.Count,
			1;                                    // CloseAngleBracketToken

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenAngleBracketToken;
		foreach (var item in GenericArgumentEntryNodeList)
		{
			childList[i++] = item;
		}
		childList[i++] = CloseAngleBracketToken;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}