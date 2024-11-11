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
	
	public ICodeBlockOwner SetOpenBraceToken(OpenBraceToken openBraceToken, IParserModel parserModel);
	public ICodeBlockOwner SetCloseBraceToken(CloseBraceToken closeBraceToken, IParserModel parserModel);
	public ICodeBlockOwner SetStatementDelimiterToken(StatementDelimiterToken statementDelimiterToken, IParserModel parserModel);
	public ICodeBlockOwner SetCodeBlockNode(CodeBlockNode codeBlockNode, IParserModel parserModel);
	
	/// <summary>
	/// Once the code block owner's scope has been constructed,
	/// this method gives them an opportunity to pull any variables
	/// into scope that ought to be there.
	/// As well, the current code block builder will have been updated.
	///
	/// (i.e.: a function definition's arguments)
	/// </summary>
	public void OnBoundScopeCreatedAndSetAsCurrent(IParserModel parserModel);
	
	public static void ThrowMultipleScopeDelimiterException(IParserModel parserModel)
	{
		// 'model.TokenWalker.Current.TextSpan' isn't necessarily the syntax passed to this method.
    	// TODO: But getting a TextSpan from a general type such as 'ISyntax' is a pain.
    	//
    	parserModel.DiagnosticBag.ReportTodoException(
    		parserModel.TokenWalker.Current.TextSpan,
    		"Scope must be set by either OpenBraceToken and CloseBraceToken; or by StatementDelimiterToken, but not both.");
	}
		
	public static void ThrowAlreadyAssignedCodeBlockNodeException(IParserModel parserModel)
	{
		// 'model.TokenWalker.Current.TextSpan' isn't necessarily the syntax passed to this method.
    	// TODO: But getting a TextSpan from a general type such as 'ISyntax' is a pain.
    	//
    	parserModel.DiagnosticBag.ReportTodoException(
    		parserModel.TokenWalker.Current.TextSpan,
    		$"The {nameof(CodeBlockNode)} was already assigned.");
	}
}
