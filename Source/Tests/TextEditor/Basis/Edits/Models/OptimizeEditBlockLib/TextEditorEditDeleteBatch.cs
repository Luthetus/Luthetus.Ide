namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

public struct TextEditorEditDeleteBatch : ITextEditorEdit
{
	public TextEditorEditDelete(int positionIndex, int count, string textDeletedBuilder)
	{
		PositionIndex = positionIndex;
		Count = count;
		TextDeletedBuilder = textDeletedBuilder;
	}

	public int PositionIndex { get; }
	public int Count { get; }
	public string TextDeletedBuilder { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.DeleteBatch;
}
