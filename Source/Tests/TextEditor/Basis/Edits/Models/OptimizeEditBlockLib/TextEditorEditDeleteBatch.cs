using System.Text;

namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

public class TextEditorEditDeleteBatch : ITextEditorEdit
{
	public TextEditorEditDeleteBatch(int positionIndex, int count, StringBuilder textRemovedBuilder)
	{
		PositionIndex = positionIndex;
		Count = count;
		TextRemovedBuilder = textRemovedBuilder;
	}

	public int PositionIndex { get; }
	public int Count { get; private set; }
	public StringBuilder TextRemovedBuilder { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.DeleteBatch;

	public void Add(int count, string textRemoved)
	{
		Count += count;
		TextRemovedBuilder.Append(textRemoved);
	}
}
