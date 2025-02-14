using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

public interface ICodeBlockOwner : ISyntaxNode
{
	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind { get; }
	public TextEditorTextSpan? OpenCodeBlockTextSpan { get; }
	public CodeBlockNode? CodeBlockNode { get;  }
	public TextEditorTextSpan? CloseCodeBlockTextSpan { get; }
	public int? ScopeIndexKey { get; set; }
	
	public TypeClauseNode? GetReturnTypeClauseNode();
	
	public ICodeBlockOwner SetOpenCodeBlockTextSpan(TextEditorTextSpan? openCodeBlockTextSpan, DiagnosticBag diagnosticBag, TokenWalker tokenWalker);
	public ICodeBlockOwner SetCloseCodeBlockTextSpan(TextEditorTextSpan? closeCodeBlockTextSpan, DiagnosticBag diagnosticBag, TokenWalker tokenWalker);
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
