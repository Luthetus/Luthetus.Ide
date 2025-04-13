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
	#endregion
}
