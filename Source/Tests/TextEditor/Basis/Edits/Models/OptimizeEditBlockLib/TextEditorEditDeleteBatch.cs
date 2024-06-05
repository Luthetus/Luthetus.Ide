using System.Text;

namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

public class TextEditorEditDeleteBatch : ITextEditorEdit
{
	public TextEditorEditDeleteBatch(int positionIndex, int count, StringBuilder textDeletedBuilder)
	{
		PositionIndex = positionIndex;
		Count = count;
		TextDeletedBuilder = textDeletedBuilder;
	}

	public int PositionIndex { get; }
	public int Count { get; private set; }
	public StringBuilder TextDeletedBuilder { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.DeleteBatch;

	public void Add(int count, string textDeleted)
	{
		Count += count;
		TextDeletedBuilder.Append(textDeleted);
	}
}
