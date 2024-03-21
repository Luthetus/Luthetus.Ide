using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Ide.RazorLib.Outputs.Models;

namespace Luthetus.Ide.RazorLib.DotNetOutputs.Models;

public record DotNetRunOutputLine(
		string SourceText,
		DotNetRunOutputKind DotNetRunOutputKind,
		TextEditorTextSpan FilePathTextSpan,
		TextEditorTextSpan RowAndColumnNumberTextSpan,
		TextEditorTextSpan ErrorKeywordAndErrorCodeTextSpan,
		TextEditorTextSpan ErrorMessageTextSpan,
		TextEditorTextSpan ProjectFilePathTextSpan)
	: IOutputLine;
