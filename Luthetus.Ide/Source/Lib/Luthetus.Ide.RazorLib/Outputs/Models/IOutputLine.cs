using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.Ide.RazorLib.Outputs.Models;

public interface IOutputLine
{
	public TextEditorTextSpan FilePathTextSpan { get; }
	public TextEditorTextSpan RowAndColumnNumberTextSpan { get; }
	public TextEditorTextSpan ErrorKeywordAndErrorCodeTextSpan { get; }
	public TextEditorTextSpan ErrorMessageTextSpan { get; }
	public TextEditorTextSpan ProjectFilePathTextSpan { get; }
}
