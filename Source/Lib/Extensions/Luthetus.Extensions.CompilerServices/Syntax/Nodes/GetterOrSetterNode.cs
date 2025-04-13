using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Utility;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class GetterOrSetterNode : ICodeBlockOwner
{
	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public TextEditorTextSpan OpenCodeBlockTextSpan { get; set; }
	public CodeBlock CodeBlock { get; set; }
	public TextEditorTextSpan CloseCodeBlockTextSpan { get; set; }
	public int ScopeIndexKey { get; set; } = -1;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.GetterOrSetterNode;
	
	#region ICodeBlockOwner_Methods
	public TypeReference GetReturnTypeReference()
	{
		return TypeFacts.Empty.ToTypeReference();
	}

	public void SetChildListIsDirty(bool childListIsDirty)
	{
		_childListIsDirty = childListIsDirty;
	}
	#endregion

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		return _childList;
	}
}

