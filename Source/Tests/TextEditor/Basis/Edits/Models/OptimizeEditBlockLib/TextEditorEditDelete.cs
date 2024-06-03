namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

public struct TextEditorEditDelete : ITextEditorEdit
{
	public TextEditorEditDelete(int positionIndex, int count)
	{
		PositionIndex = positionIndex;
		Count = count;
	}

	public int PositionIndex { get; }
	public int Count { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.Delete;
}
