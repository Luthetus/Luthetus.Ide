using System.Text;

namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

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
