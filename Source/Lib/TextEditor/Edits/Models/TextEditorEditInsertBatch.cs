using System.Text;

namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public class TextEditorEditInsertBatch : ITextEditorEdit
{
	public TextEditorEditInsertBatch(int positionIndex, StringBuilder contentBuilder)
	{
		PositionIndex = positionIndex;
		ContentBuilder = contentBuilder;
	}

	public int PositionIndex { get; }
	public StringBuilder ContentBuilder { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.InsertBatch;
}
