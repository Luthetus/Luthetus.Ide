using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.Extensions.CompilerServices.Utility;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class DoWhileStatementNode : ICodeBlockOwner
{
	public DoWhileStatementNode(
		SyntaxToken doKeywordToken,
		SyntaxToken whileKeywordToken,
		SyntaxToken openParenthesisToken,
		IExpressionNode? expressionNode,
		SyntaxToken closeParenthesisToken)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.DoWhileStatementNode++;
		#endif
	
		DoKeywordToken = doKeywordToken;
		WhileKeywordToken = whileKeywordToken;
		OpenParenthesisToken = openParenthesisToken;
		ExpressionNode = expressionNode;
		CloseParenthesisToken = closeParenthesisToken;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken DoKeywordToken { get; }
	public SyntaxToken WhileKeywordToken { get; private set; }
	public SyntaxToken OpenParenthesisToken { get; private set; }
	public IExpressionNode? ExpressionNode { get; set; }
	public SyntaxToken CloseParenthesisToken { get; private set; }

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public TextEditorTextSpan OpenCodeBlockTextSpan { get; set; }
	public CodeBlock CodeBlock { get; set; }
	public TextEditorTextSpan CloseCodeBlockTextSpan { get; set; }
	public int ScopeIndexKey { get; set; } = -1;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.DoWhileStatementNode;

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

		var childCount = 1; // DoKeywordToken,
		// if (CodeBlockNode is not null)
		// 	childCount++;
		if (WhileKeywordToken.ConstructorWasInvoked)
			childCount++;
		if (OpenParenthesisToken.ConstructorWasInvoked)
			childCount++;
		if (ExpressionNode is not null)
			childCount++;
		if (CloseParenthesisToken.ConstructorWasInvoked)
			childCount++;

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = DoKeywordToken;
		// if (CodeBlockNode is not null)
		// 	childList[i++] = CodeBlockNode;
		if (WhileKeywordToken.ConstructorWasInvoked)
			childList[i++] = WhileKeywordToken;
		if (OpenParenthesisToken.ConstructorWasInvoked)
			childList[i++] = OpenParenthesisToken;
		if (ExpressionNode is not null)
			childList[i++] = ExpressionNode;
		if (CloseParenthesisToken.ConstructorWasInvoked)
			childList[i++] = CloseParenthesisToken;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}

	public void SetWhileProperties(
		SyntaxToken whileKeywordToken,
		SyntaxToken openParenthesisToken,
		IExpressionNode expressionNode,
		SyntaxToken closeParenthesisToken)
	{
		WhileKeywordToken = whileKeywordToken;
		OpenParenthesisToken = openParenthesisToken;
		ExpressionNode = expressionNode;
		CloseParenthesisToken = closeParenthesisToken;

		_childListIsDirty = true;
	}
}
