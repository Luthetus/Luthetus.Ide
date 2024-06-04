namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

public struct TextEditorEditDelete : ITextEditorEdit
{
	public TextEditorEditDelete(int positionIndex, int count, string textDeleted)
	{
		PositionIndex = positionIndex;
		Count = count;
		TextDeleted = textDeleted;
	}

	public int PositionIndex { get; }
	public int Count { get; }
	public string TextDeleted { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.Delete;
}
