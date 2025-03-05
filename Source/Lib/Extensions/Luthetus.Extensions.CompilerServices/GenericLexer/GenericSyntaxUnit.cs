using Luthetus.Extensions.CompilerServices.GenericLexer.SyntaxObjects;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.Extensions.CompilerServices.GenericLexer;

public class GenericSyntaxUnit
{
	public GenericSyntaxUnit(
		GenericDocumentSyntax genericDocumentSyntax,
		List<TextEditorDiagnostic> diagnosticList)
	{
		GenericDocumentSyntax = genericDocumentSyntax;
		DiagnosticList = diagnosticList;
	}

	public GenericDocumentSyntax GenericDocumentSyntax { get; }
	public IReadOnlyList<TextEditorDiagnostic> DiagnosticList { get; }
}