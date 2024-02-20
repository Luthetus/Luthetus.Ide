using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.Ide.Tests.Basis.Outputs.Models;

public record DotNetRunOutputLineTests(
		string SourceText,
		TextEditorTextSpan FilePathTextSpan,
		TextEditorTextSpan RowAndColumnNumberTextSpan,
		TextEditorTextSpan ErrorKeywordAndErrorCodeTextSpan,
		TextEditorTextSpan ErrorMessageTextSpan,
		TextEditorTextSpan ProjectFilePathTextSpan);
