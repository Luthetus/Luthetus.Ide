using System.Text;

namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public struct TextEditorEditDelete : ITextEditorEdit
{
	public TextEditorEditDelete(int positionIndex, int count, StringBuilder textRemovedBuilder)
	{
		PositionIndex = positionIndex;
		Count = count;
		TextRemovedBuilder = textRemovedBuilder;
	}

	public int PositionIndex { get; }
	public int Count { get; private set; }
	public StringBuilder TextRemovedBuilder { get; }

	/// <summary>
	/// TODO: Consider storing the text that was inserted/deleted in a shared List,...
	/// ...and each edit stores the indices at which the text it altered exists.
	/// </summary>
	public string? TextRemoved { get; set; }

	public TextEditorEditKind EditKind => TextEditorEditKind.Delete;
	
	public void Add(int count, string textRemoved)
	{
		Count += count;
		TextRemovedBuilder.Append(textRemoved);
	}
}
