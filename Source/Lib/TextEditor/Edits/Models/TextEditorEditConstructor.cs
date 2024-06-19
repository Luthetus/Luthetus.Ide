namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public struct TextEditorEditConstructor : ITextEditorEdit
{
	public TextEditorEditKind EditKind => TextEditorEditKind.Constructor;
}
