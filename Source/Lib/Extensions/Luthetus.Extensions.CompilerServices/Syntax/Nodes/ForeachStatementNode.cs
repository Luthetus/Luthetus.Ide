using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.Extensions.CompilerServices.Utility;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class ForeachStatementNode : ICodeBlockOwner
{
	public ForeachStatementNode(
		SyntaxToken foreachKeywordToken,
		SyntaxToken openParenthesisToken,
		VariableDeclarationNode variableDeclarationNode,
		SyntaxToken inKeywordToken,
		IExpressionNode expressionNode,
		SyntaxToken closeParenthesisToken,
		CodeBlockNode? codeBlockNode)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ForeachStatementNode++;
		#endif
	
		ForeachKeywordToken = foreachKeywordToken;
		OpenParenthesisToken = openParenthesisToken;
		VariableDeclarationNode = variableDeclarationNode;
		InKeywordToken = inKeywordToken;
		ExpressionNode = expressionNode;
		CloseParenthesisToken = closeParenthesisToken;
		CodeBlockNode = codeBlockNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken ForeachKeywordToken { get; }
	public SyntaxToken OpenParenthesisToken { get; }
	public VariableDeclarationNode VariableDeclarationNode { get; }
	public SyntaxToken InKeywordToken { get; }
	public IExpressionNode ExpressionNode { get; }
	public SyntaxToken CloseParenthesisToken { get; }

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public TextEditorTextSpan OpenCodeBlockTextSpan { get; set; }
	public CodeBlockNode? CodeBlockNode { get; set; }
	public TextEditorTextSpan CloseCodeBlockTextSpan { get; set; }
	public int ScopeIndexKey { get; set; } = -1;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ForeachStatementNode;

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

		var childCount = 6; // ForeachKeywordToken, OpenParenthesisToken, VariableDeclarationNode, InKeywordToken, ExpressionNode, CloseParenthesisToken,
		if (OpenParenthesisToken.ConstructorWasInvoked)
			childCount++;
		if (CodeBlockNode is not null)
			childCount++;

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = ForeachKeywordToken;
		childList[i++] = OpenParenthesisToken;
		childList[i++] = VariableDeclarationNode;
		childList[i++] = InKeywordToken;
		childList[i++] = ExpressionNode;
		childList[i++] = CloseParenthesisToken;
		if (OpenParenthesisToken.ConstructorWasInvoked)
			childList[i++] = OpenParenthesisToken;
		if (CodeBlockNode is not null)
			childList[i++] = CodeBlockNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}
