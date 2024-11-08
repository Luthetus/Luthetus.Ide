using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Exceptions;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

public interface ICodeBlockOwner : ISyntaxNode
{
	public ScopeDirectionKind ScopeDirectionKind { get; }
	public OpenBraceToken OpenBraceToken { get; }
	public CloseBraceToken CloseBraceToken { get; }
	public StatementDelimiterToken StatementDelimiterToken { get; }
	public CodeBlockNode? CodeBlockNode { get; }
	public bool IsSingleStatementBody { get; }
	
	public TypeClauseNode? GetReturnTypeClauseNode();
	
	public ICodeBlockOwner SetOpenBraceToken(OpenBraceToken openBraceToken);
	public ICodeBlockOwner SetCloseBraceToken(CloseBraceToken closeBraceToken);
	public ICodeBlockOwner SetStatementDelimiterToken(StatementDelimiterToken statementDelimiterToken);
	public ICodeBlockOwner SetCodeBlockNode(CodeBlockNode codeBlockNode);
	
	/// <summary>
	/// Once the code block owner's scope has been constructed,
	/// this method gives them an opportunity to pull any variables
	/// into scope that ought to be there.
	/// As well, the current code block builder will have been updated.
	///
	/// (i.e.: a function definition's arguments)
	/// </summary>
	public void OnBoundScopeCreatedAndSetAsCurrent(IParserModel parserModel);
	
	public static void ThrowMultipleScopeDelimiterException() =>
		throw new LuthetusTextEditorException("Scope must be set by either OpenBraceToken and CloseBraceToken; or by StatementDelimiterToken, but not both.");
		
	public static void ThrowAlreadyAssignedCodeBlockNodeException() =>
		throw new LuthetusTextEditorException($"The {nameof(CodeBlockNode)} was already assigned.");
}
