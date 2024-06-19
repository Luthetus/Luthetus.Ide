using System.Text;

namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public class TextEditorEditBackspaceBatch : ITextEditorEdit
{
	public TextEditorEditBackspaceBatch(int positionIndex, int count, StringBuilder textRemovedBuilder)
	{
		PositionIndex = positionIndex;
		Count = count;
		TextRemovedBuilder = textRemovedBuilder;
	}

	public int PositionIndex { get; }
	public int Count { get; private set; }
	public StringBuilder TextRemovedBuilder { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.BackspaceBatch;

	public void Add(int count, string textRemoved)
	{
		Count += count;
		TextRemovedBuilder.Insert(0, textRemoved);
	}
}
