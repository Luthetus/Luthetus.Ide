namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// Examples:<br/>
/// 
/// public T Clone&lt;T&gt;(T item) where T : class<br/>
/// {<br/>
/// &#9;return item;<br/>
/// }<br/>
/// 
/// public T Clone&lt;T&gt;(T item) where T : class => item;<br/>
/// </summary>
public sealed class ConstraintNode : ISyntaxNode
{
	public ConstraintNode(IReadOnlyList<SyntaxToken> innerTokens)
	{
		// Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ConstraintNode++;
	
		InnerTokens = innerTokens;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	/// <summary>
	/// TODO: For now, just grab all tokens and put them in an array...
	/// ...In the future parse the tokens. (2023-10-19)
	/// </summary>
	public IReadOnlyList<SyntaxToken> InnerTokens { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ConstraintNode;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		_childList = InnerTokens.Select(x => (ISyntax)x).ToArray();

		_childListIsDirty = false;
		return _childList;
	}
}