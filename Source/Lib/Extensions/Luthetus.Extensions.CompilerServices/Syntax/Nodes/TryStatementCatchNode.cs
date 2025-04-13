using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Utility;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class TryStatementCatchNode : ICodeBlockOwner
{
	public TryStatementCatchNode(
		TryStatementNode? parent,
		SyntaxToken keywordToken,
		SyntaxToken openParenthesisToken,
		SyntaxToken closeParenthesisToken,
		CodeBlock codeBlock)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.TryStatementCatchNode++;
		#endif
	
		Parent = parent;
		KeywordToken = keywordToken;
		CodeBlock = codeBlock;
		OpenParenthesisToken = openParenthesisToken;
		CloseParenthesisToken = closeParenthesisToken;
		CodeBlock = codeBlock;
	}

	// private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken KeywordToken { get; }
	public SyntaxToken OpenParenthesisToken { get; }
	public VariableDeclarationNode? VariableDeclarationNode { get; private set; }
	public SyntaxToken CloseParenthesisToken { get; }

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public TextEditorTextSpan OpenCodeBlockTextSpan { get; set; }
	public CodeBlock CodeBlock { get; set; }
	public TextEditorTextSpan CloseCodeBlockTextSpan { get; set; }
	public int ScopeIndexKey { get; set; } = -1;

	public ISyntaxNode? Parent { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.TryStatementCatchNode;

	public TryStatementCatchNode SetVariableDeclarationNode(VariableDeclarationNode variableDeclarationNode)
	{
		VariableDeclarationNode = variableDeclarationNode;
		_childListIsDirty = true;
		return this;
	}

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

	/*public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 0;
		if (KeywordToken.ConstructorWasInvoked)
			childCount++;
		// if (CodeBlockNode is not null)
		//	childCount++;
		if (OpenParenthesisToken.ConstructorWasInvoked)
			childCount++;
		if (CloseParenthesisToken.ConstructorWasInvoked)
			childCount++;
		// if (CodeBlockNode is not null)
		// 	childCount++;

		var childList = new ISyntax[childCount];
		var i = 0;

		if (KeywordToken.ConstructorWasInvoked)
			childList[i++] = KeywordToken;
		// if (CodeBlockNode is not null)
		// 	childList[i++] = CodeBlockNode;
		if (OpenParenthesisToken.ConstructorWasInvoked)
			childList[i++] = OpenParenthesisToken;
		if (CloseParenthesisToken.ConstructorWasInvoked)
			childList[i++] = CloseParenthesisToken;
		// if (CodeBlockNode is not null)
		// 	childList[i++] = CodeBlockNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}*/
}
