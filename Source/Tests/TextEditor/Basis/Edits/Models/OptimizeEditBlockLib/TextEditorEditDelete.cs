namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

public struct TextEditorEditDelete : ITextEditorEdit
{
	public TextEditorEditDelete(int positionIndex, int count)
	{
		PositionIndex = positionIndex;
		Count = count;
		TextRemoved = string.Empty;
	}

	public int PositionIndex { get; }
	public int Count { get; }

	// TODO: Use Span<T>
	public string? TextRemoved { get; set; }

	public TextEditorEditKind EditKind => TextEditorEditKind.Delete;
}