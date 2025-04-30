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

	public SyntaxToken DoKeywordToken { get; }
	public SyntaxToken WhileKeywordToken { get; set; }
	public SyntaxToken OpenParenthesisToken { get; set; }
	public IExpressionNode? ExpressionNode { get; set; }
	public SyntaxToken CloseParenthesisToken { get; set; }

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
	#endregion

	#if DEBUG	
	~DoWhileStatementNode()
	{
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.DoWhileStatementNode--;
	}
	#endif
}
