using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
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
	
	public ICodeBlockOwner SetOpenBraceToken(OpenBraceToken openBraceToken, DiagnosticBag diagnosticBag, TokenWalker tokenWalker);
	public ICodeBlockOwner SetCloseBraceToken(CloseBraceToken closeBraceToken, DiagnosticBag diagnosticBag, TokenWalker tokenWalker);
	public ICodeBlockOwner SetStatementDelimiterToken(StatementDelimiterToken statementDelimiterToken, DiagnosticBag diagnosticBag, TokenWalker tokenWalker);
	public ICodeBlockOwner SetCodeBlockNode(CodeBlockNode codeBlockNode, DiagnosticBag diagnosticBag, TokenWalker tokenWalker);
	
	public static void ThrowMultipleScopeDelimiterException(DiagnosticBag diagnosticBag, TokenWalker tokenWalker)
	{
		// 'model.TokenWalker.Current.TextSpan' isn't necessarily the syntax passed to this method.
    	// TODO: But getting a TextSpan from a general type such as 'ISyntax' is a pain.
    	//
    	diagnosticBag.ReportTodoException(
    		tokenWalker.Current.TextSpan,
    		"Scope must be set by either OpenBraceToken and CloseBraceToken; or by StatementDelimiterToken, but not both.");
	}
		
	public static void ThrowAlreadyAssignedCodeBlockNodeException(DiagnosticBag diagnosticBag, TokenWalker tokenWalker)
	{
		// 'model.TokenWalker.Current.TextSpan' isn't necessarily the syntax passed to this method.
    	// TODO: But getting a TextSpan from a general type such as 'ISyntax' is a pain.
    	//
    	diagnosticBag.ReportTodoException(
    		tokenWalker.Current.TextSpan,
    		$"The {nameof(CodeBlockNode)} was already assigned.");
	}
}
