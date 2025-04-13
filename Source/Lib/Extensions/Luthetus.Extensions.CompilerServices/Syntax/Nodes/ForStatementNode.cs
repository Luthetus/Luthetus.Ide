using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.Extensions.CompilerServices.Utility;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class ForStatementNode : ICodeBlockOwner
{
	public ForStatementNode(
		SyntaxToken keywordToken,
		SyntaxToken openParenthesisToken,
		IReadOnlyList<ISyntax> initializationSyntaxList,
		SyntaxToken initializationStatementDelimiterToken,
		IExpressionNode conditionExpressionNode,
		SyntaxToken conditionStatementDelimiterToken,
		IExpressionNode updationExpressionNode,
		SyntaxToken closeParenthesisToken,
		CodeBlock codeBlock)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ForStatementNode++;
		#endif
	
		KeywordToken = keywordToken;
		OpenParenthesisToken = openParenthesisToken;
		InitializationSyntaxList = initializationSyntaxList;
		InitializationStatementDelimiterToken = initializationStatementDelimiterToken;
		ConditionExpressionNode = conditionExpressionNode;
		ConditionStatementDelimiterToken = conditionStatementDelimiterToken;
		UpdationExpressionNode = updationExpressionNode;
		CloseParenthesisToken = closeParenthesisToken;
		CodeBlock = codeBlock;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken KeywordToken { get; }
	public SyntaxToken OpenParenthesisToken { get; }
	public IReadOnlyList<ISyntax> InitializationSyntaxList { get; }
	public SyntaxToken InitializationStatementDelimiterToken { get; }
	public IExpressionNode ConditionExpressionNode { get; }
	public SyntaxToken ConditionStatementDelimiterToken { get; }
	public IExpressionNode UpdationExpressionNode { get; }
	public SyntaxToken CloseParenthesisToken { get; }

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public TextEditorTextSpan OpenCodeBlockTextSpan { get; set; }
	public CodeBlock CodeBlock { get; set; }
	public TextEditorTextSpan CloseCodeBlockTextSpan { get; set; }
	public int ScopeIndexKey { get; set; } = -1;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ForStatementNode;

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

		// KeywordToken, OpenParenthesisToken, InitializationSyntaxList.Length, InitializationStatementDelimiterToken,
		// ConditionExpressionNode, ConditionStatementDelimiterToken, UpdationExpressionNode, CloseParenthesisToken,
		var childCount =
			1 +                               // KeywordToken,
			1 +                               // OpenParenthesisToken,
			InitializationSyntaxList.Count +  // InitializationSyntaxList.Length
			1 +                               // InitializationStatementDelimiterToken,
			1 +                               // ConditionExpressionNode,
			1 +                               // ConditionStatementDelimiterToken,
			1 +                               // UpdationExpressionNode,
			1;                                // CloseParenthesisToken,

		// if (CodeBlockNode is not null)
		// 	childCount++;

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = KeywordToken;
		childList[i++] = OpenParenthesisToken;
		foreach (var item in InitializationSyntaxList)
		{
			childList[i++] = item;
		}
		childList[i++] = InitializationStatementDelimiterToken;
		childList[i++] = ConditionExpressionNode;
		childList[i++] = ConditionStatementDelimiterToken;
		childList[i++] = UpdationExpressionNode;
		childList[i++] = CloseParenthesisToken;
		// if (CodeBlockNode is not null)
		// 	childList[i++] = CodeBlockNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}
