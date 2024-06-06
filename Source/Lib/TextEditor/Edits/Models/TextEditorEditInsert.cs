namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public struct TextEditorEditInsert : ITextEditorEdit
{
	public TextEditorEditInsert(int positionIndex, string content)
	{
		PositionIndex = positionIndex;
		Content = content;
	}

	public int PositionIndex { get; }
	public string Content { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.Insert;
}


