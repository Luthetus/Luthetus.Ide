using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Utility;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class ArbitraryCodeBlockNode : ICodeBlockOwner
{
	public ArbitraryCodeBlockNode(ICodeBlockOwner parentCodeBlockOwner)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ArbitraryCodeBlockNode++;
		#endif
	
		ParentCodeBlockOwner = parentCodeBlockOwner;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public ICodeBlockOwner ParentCodeBlockOwner { get; }

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ParentCodeBlockOwner.ScopeDirectionKind;
	public TextEditorTextSpan OpenCodeBlockTextSpan { get; set; }
	public CodeBlock CodeBlock { get; set; }
	public TextEditorTextSpan CloseCodeBlockTextSpan { get; set; }
	public int ScopeIndexKey { get; set; } = -1;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ArbitraryCodeBlockNode;

	#region ICodeBlockOwner_Methods
	public TypeReference GetReturnTypeReference()
	{
		if (ParentCodeBlockOwner is null)
			return TypeFacts.Empty.ToTypeReference();
		
		return ParentCodeBlockOwner.GetReturnTypeReference();
	}

	public void SetChildListIsDirty(bool childListIsDirty)
	{
		_childListIsDirty = childListIsDirty;
	}
	#endregion

	public IReadOnlyList<ISyntax> GetChildList() => CodeBlock.ChildList;
}
