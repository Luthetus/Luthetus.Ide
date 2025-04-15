using System.Text;

namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public struct TextEditorEditInsert : ITextEditorEdit
{
	public TextEditorEditInsert(int positionIndex, StringBuilder contentBuilder)
	{
		PositionIndex = positionIndex;
		ContentBuilder = contentBuilder;
	}

	public int PositionIndex { get; }
	
	/// <summary>
	/// Consider storing the text that was inserted/deleted in a shared List,...
	/// ...and each edit stores the indices at which the text it altered exists.
	/// </summary>
	public StringBuilder ContentBuilder { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.Insert;
}
