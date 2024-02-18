using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.Ide.RazorLib.Outputs.Models;

public record DotNetRunOutputLine(
		TextEditorTextSpan FilePathTextSpan,
		TextEditorTextSpan RowAndColumnNumberTextSpan,
		TextEditorTextSpan ErrorKeywordAndErrorCodeTextSpan,
		TextEditorTextSpan ErrorMessageTextSpan,
		TextEditorTextSpan ProjectFilePathTextSpan)
	: IOutputLine;
