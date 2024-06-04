using System.Text;

namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

public class TextEditorEditBackspaceBatch : ITextEditorEdit
{
	public TextEditorEditBackspaceBatch(int positionIndex, int count, StringBuilder textDeletedBuilder)
	{
		PositionIndex = positionIndex;
		Count = count;
		TextDeletedBuilder = textDeletedBuilder;
	}

	public int PositionIndex { get; }
	public int Count { get; }
	public StringBuilder TextDeletedBuilder { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.BackspaceBatch;
}
