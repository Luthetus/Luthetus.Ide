using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class CompilationUnit : ICompilationUnit
{
	public IReadOnlyList<TextEditorDiagnostic> DiagnosticList { get; init; } = Array.Empty<TextEditorDiagnostic>();

	public IEnumerable<TextEditorTextSpan> GetTextTextSpans()
	{
		return Array.Empty<TextEditorTextSpan>();
	}

	public IEnumerable<TextEditorTextSpan> GetDiagnosticTextSpans()
	{
		return DiagnosticList.Select(x => x.TextSpan);
	}
}
