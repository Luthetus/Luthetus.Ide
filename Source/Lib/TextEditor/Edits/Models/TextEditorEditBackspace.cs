using System.Text;

namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public struct TextEditorEditBackspace : ITextEditorEdit
{
	public TextEditorEditBackspace(int positionIndex, int count, StringBuilder textRemovedBuilder)
	{
		PositionIndex = positionIndex;
		Count = count;
		TextRemovedBuilder = textRemovedBuilder;
	}

	public int PositionIndex { get; }
	public int Count { get; private set; }
	
	/// <summary>
	/// Consider storing the text that was inserted/deleted in a shared List,...
	/// ...and each edit stores the indices at which the text it altered exists.
	/// </summary>
	public StringBuilder TextRemovedBuilder { get; }

	public TextEditorEditKind EditKind => TextEditorEditKind.Backspace;
	
	public void Add(int count, string textRemoved)
	{
		Count += count;
		TextRemovedBuilder.Insert(0, textRemoved);
	}
}
