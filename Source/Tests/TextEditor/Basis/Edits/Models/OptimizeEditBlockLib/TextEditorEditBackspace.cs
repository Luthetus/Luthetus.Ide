namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

public struct TextEditorEditBackspace : ITextEditorEdit
{
	public TextEditorEditBackspace(int positionIndex, int count, string textDeleted)
	{
		PositionIndex = positionIndex;
		Count = count;
		TextDeleted = textDeleted;
	}

	public int PositionIndex { get; }
	public int Count { get; }
	public string TextDeleted { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.Backspace;
}
