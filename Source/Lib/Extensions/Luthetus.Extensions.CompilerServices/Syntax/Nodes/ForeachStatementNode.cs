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
		CodeBlock codeBlock)
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
		CodeBlock = codeBlock;
	}

	public SyntaxToken ForeachKeywordToken { get; }
	public SyntaxToken OpenParenthesisToken { get; }
	public VariableDeclarationNode VariableDeclarationNode { get; }
	public SyntaxToken InKeywordToken { get; }
	public IExpressionNode ExpressionNode { get; }
	public SyntaxToken CloseParenthesisToken { get; }

	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;
	public TextEditorTextSpan OpenCodeBlockTextSpan { get; set; }
	public CodeBlock CodeBlock { get; set; }
	public TextEditorTextSpan CloseCodeBlockTextSpan { get; set; }
	public int ScopeIndexKey { get; set; } = -1;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ForeachStatementNode;

	#region ICodeBlockOwner_Methods
	public TypeReference GetReturnTypeReference()
	{
		return TypeFacts.Empty.ToTypeReference();
	}
	#endregion
}
