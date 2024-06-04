namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

public struct TextEditorEditBackspaceBatch : ITextEditorEdit
{
	public TextEditorEditBackspace(int positionIndex, int count, StringBuilder textDeletedBuilder)
	{
		PositionIndex = positionIndex;
		Count = count;
		TextDeletedBuilder = textDeletedBuilder;
	}

	public int PositionIndex { get; }
	public int Count { get; }
	public string TextDeletedBuilder { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.BackspaceBatch;
}
