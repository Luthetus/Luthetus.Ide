using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Utility;
using Luthetus.Extensions.CompilerServices.Syntax;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

public interface ICodeBlockOwner : ISyntaxNode
{
	// ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind { get; }
	public TextEditorTextSpan? OpenCodeBlockTextSpan { get; }
	public CodeBlockNode? CodeBlockNode { get; }
	public TextEditorTextSpan? CloseCodeBlockTextSpan { get; }
	
	/// <summary>
	/// This needs to not be nullable due to the cost of boxing combined with
	/// the frequency that this property's value is read.
	/// </summary>
	public int? ScopeIndexKey { get; set; }

	public TypeClauseNode? GetReturnTypeClauseNode();

	public ICodeBlockOwner SetOpenCodeBlockTextSpan(TextEditorTextSpan? openCodeBlockTextSpan, List<TextEditorDiagnostic> diagnosticList, TokenWalker tokenWalker);
	public ICodeBlockOwner SetCloseCodeBlockTextSpan(TextEditorTextSpan? closeCodeBlockTextSpan, List<TextEditorDiagnostic> diagnosticList, TokenWalker tokenWalker);
	public ICodeBlockOwner SetCodeBlockNode(CodeBlockNode codeBlockNode, List<TextEditorDiagnostic> diagnosticList, TokenWalker tokenWalker);

	public static void ThrowMultipleScopeDelimiterException(List<TextEditorDiagnostic> diagnosticList, TokenWalker tokenWalker)
	{
		// 'model.TokenWalker.Current.TextSpan' isn't necessarily the syntax passed to this method.
		// TODO: But getting a TextSpan from a general type such as 'ISyntax' is a pain.
		//
		/*diagnosticBag.ReportTodoException(
    		tokenWalker.Current.TextSpan,
    		"Scope must be set by either OpenBraceToken and CloseBraceToken; or by StatementDelimiterToken, but not both.");*/
	}

	public static void ThrowAlreadyAssignedCodeBlockNodeException(List<TextEditorDiagnostic> diagnosticList, TokenWalker tokenWalker)
	{
		// 'model.TokenWalker.Current.TextSpan' isn't necessarily the syntax passed to this method.
		// TODO: But getting a TextSpan from a general type such as 'ISyntax' is a pain.
		//
		/*diagnosticBag.ReportTodoException(
    		tokenWalker.Current.TextSpan,
    		$"The {nameof(CodeBlockNode)} was already assigned.");*/
	}
}
