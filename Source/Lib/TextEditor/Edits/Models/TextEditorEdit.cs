using System.Text;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Cursors.Models;

namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public struct TextEditorEdit
{
	public TextEditorEdit(TextEditorEditKind editKind, string tag, TextEditorCursor cursor, int positionIndex, StringBuilder? contentBuilder)
	{
		EditKind = editKind;
		Tag = tag;
		Cursor = cursor;
		PositionIndex = positionIndex;
		ContentBuilder = contentBuilder;
	}

	public TextEditorEditKind EditKind { get; }
	public string Tag { get; } = string.Empty;
	public int PositionIndex { get; }
	
	/// <summary>
	/// Whether the 'Cursor' position index is equal to the 'PositionIndex' property on this type,
	/// I'm not sure. So I'm keeping the separate 'PositionIndex' for now.
	/// </summary>
	public TextEditorCursor Cursor { get; }
	
	/// <summary>
	/// The TextEditorEditKind(s) { Constructor, Other } will have a null ContentBuilder.
	///
	/// All other TextEditorEditKind(s) are presumed to NOT be null.
	///
	/// TODO: (optimization) Consider storing the text that was inserted/deleted in a shared List,...
	/// ...and each edit stores the indices at which the text it altered exists.
	/// </summary>
	public StringBuilder? ContentBuilder { get; }
	
	public void Add(string text)
	{
		switch (EditKind)
		{
			case TextEditorEditKind.Insert:
				ContentBuilder!.Append(text);
				break;
			case TextEditorEditKind.Delete:
				ContentBuilder!.Append(text);
				break;
			case TextEditorEditKind.Backspace:
				ContentBuilder!.Insert(0, text);
				break;
			case TextEditorEditKind.Constructor:
				throw new LuthetusTextEditorException($"The {nameof(TextEditorEditKind)}: {EditKind}, cannot be un-done. This edit represents the initial state.");
			case TextEditorEditKind.Other:
				throw new NotImplementedException("TODO: TextEditorEditKind.Other");
			default:
				throw new NotImplementedException($"The {nameof(TextEditorEditKind)}: {EditKind} was not recognized.");
		}
	}

	public TextEditorEdit ToUndo()
	{
		switch (EditKind)
		{
			case TextEditorEditKind.Insert:
				return new TextEditorEdit(
					TextEditorEditKind.Delete,
					tag: string.Empty,
					Cursor,
					PositionIndex,
					ContentBuilder);
			case TextEditorEditKind.Backspace:
				return new TextEditorEdit(
					TextEditorEditKind.Insert,
					tag: string.Empty,
					Cursor,
					PositionIndex - ContentBuilder.Length,
					ContentBuilder);
			case TextEditorEditKind.Delete:
				return new TextEditorEdit(
					TextEditorEditKind.Insert,
					tag: string.Empty,
					Cursor,
					PositionIndex,
					ContentBuilder);
			case TextEditorEditKind.Constructor:
				throw new LuthetusTextEditorException($"The {nameof(TextEditorEditKind)}: {EditKind}, cannot be un-done. This edit represents the initial state.");
			case TextEditorEditKind.Other:
				// Other will return itself, and this signals to the undo code to enter a while loop
				// and continually undo until it encounters a different Other edit with a matching Tag.
				return this;
			default:
				throw new NotImplementedException($"The {nameof(TextEditorEditKind)}: {EditKind} was not recognized.");
		}
	}
}
