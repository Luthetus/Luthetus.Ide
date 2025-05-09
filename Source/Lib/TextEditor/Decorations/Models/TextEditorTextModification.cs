using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.Decorations.Models;

public struct TextEditorTextModification
{
	public TextEditorTextModification(bool wasInsertion, TextEditorTextSpan textEditorTextSpan)
	{
		WasInsertion = wasInsertion;
		TextEditorTextSpan = textEditorTextSpan;
	}
	
	public bool WasInsertion;
	public TextEditorTextSpan TextEditorTextSpan;
}
