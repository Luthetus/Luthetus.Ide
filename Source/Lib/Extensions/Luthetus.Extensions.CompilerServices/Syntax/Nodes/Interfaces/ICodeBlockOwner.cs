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
	/// The initializer for this should set it to '-1'
	/// to signify that the scope was not yet assigned.
	///
	/// This is unfortunate, since the default value of 'int' is '0',
	/// then this is sort of "omnipotent information" that absolutely would
	/// cause a bug if someone were to miss this step when
	/// creating a new implementation of the 'ICodeBlockOwner'.
	/// 
	/// Initially this was a nullable int, in order to ensure the implementations
	/// of 'ICodeBlockOwner', didn't have to initialize the int to '-1'.
	///
	/// The issue with the nullable approach, is that whenever you are parsing
	/// and want to change scope, you're then reading a nullable int,
	/// which results in the int being boxed.
	/// </summary>
	public int ScopeIndexKey { get; set; }

	public TypeReference GetReturnTypeReference();

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
