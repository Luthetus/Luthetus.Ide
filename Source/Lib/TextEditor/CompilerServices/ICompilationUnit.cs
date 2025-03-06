using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public interface ICompilationUnit
{
	public IEnumerable<TextEditorTextSpan> GetTextTextSpans();
	public IEnumerable<TextEditorTextSpan> GetDiagnosticTextSpans();
}
