namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

public struct TextEditorEditBackspace : ITextEditorEdit
{
	public TextEditorEditBackspace(int positionIndex, int count)
	{
		PositionIndex = positionIndex;
		Count = count;
	}

	public int PositionIndex { get; }
	public int Count { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.Backspace;
}
