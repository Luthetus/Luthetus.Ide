using System.Text;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Cursors.Models;

namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public struct TextEditorEdit
{
	public TextEditorEdit(
		TextEditorEditKind editKind,
		string tag,
		int beforePositionIndex,
		TextEditorCursor beforeCursor,
		TextEditorCursor afterCursor,
		StringBuilder? editedTextBuilder)
	{
		EditKind = editKind;
		Tag = tag;
		BeforePositionIndex = beforePositionIndex;
		BeforeCursor = beforeCursor;
		AfterCursor = afterCursor;
		EditedTextBuilder = editedTextBuilder;
	}

	public TextEditorEditKind EditKind { get; }
	public string Tag { get; } = string.Empty;
	
	public int BeforePositionIndex { get; }
	
	/// <summary>
	/// Whether the 'Cursor' position index is equal to the 'BeforePositionIndex' property on this type,
	/// I'm not sure. So I'm keeping the separate 'BeforePositionIndex' for now.
	/// </summary>
	public TextEditorCursor BeforeCursor { get; }
	
	public TextEditorCursor AfterCursor { get; }
	
	/// <summary>
	/// The TextEditorEditKind(s) { Constructor, Other } will have a null EditedTextBuilder.
	///
	/// All other TextEditorEditKind(s) are presumed to NOT be null.
	///
	/// TODO: (optimization) Consider storing the text that was inserted/deleted in a shared List,...
	/// ...and each edit stores the indices at which the text it altered exists.
	/// </summary>
	public StringBuilder? EditedTextBuilder { get; }
	
	public void Add(string text)
	{
		switch (EditKind)
		{
			case TextEditorEditKind.Insert:
				EditedTextBuilder!.Append(text);
				break;
			case TextEditorEditKind.Delete:
				EditedTextBuilder!.Append(text);
				break;
			case TextEditorEditKind.Backspace:
				EditedTextBuilder!.Insert(0, text);
				break;
			case TextEditorEditKind.Constructor:
				throw new LuthetusTextEditorException($"The {nameof(TextEditorEditKind)}: {EditKind}, cannot be un-done. This edit represents the initial state.");
			case TextEditorEditKind.OtherOpen:
				throw new NotImplementedException("TODO: TextEditorEditKind.OtherOpen");
			case TextEditorEditKind.OtherClose:
				throw new NotImplementedException("TODO: TextEditorEditKind.OtherClose");
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
					BeforePositionIndex,
					BeforeCursor,
					AfterCursor,
					EditedTextBuilder);
			case TextEditorEditKind.Backspace:
				return new TextEditorEdit(
					TextEditorEditKind.Insert,
					tag: string.Empty,
					BeforePositionIndex - EditedTextBuilder.Length,
					BeforeCursor,
					AfterCursor,
					EditedTextBuilder);
			case TextEditorEditKind.Delete:
				return new TextEditorEdit(
					TextEditorEditKind.Insert,
					tag: string.Empty,
					BeforePositionIndex,
					BeforeCursor,
					AfterCursor,
					EditedTextBuilder);
			case TextEditorEditKind.Constructor:
				throw new LuthetusTextEditorException($"The {nameof(TextEditorEditKind)}: {EditKind}, cannot be un-done. This edit represents the initial state.");
			case TextEditorEditKind.OtherOpen:
			case TextEditorEditKind.OtherClose:
				// Other will return itself, and this signals to the undo code to enter a while loop
				// and continually undo until it encounters a different Other edit with a matching Tag.
				return this;
			default:
				throw new NotImplementedException($"The {nameof(TextEditorEditKind)}: {EditKind} was not recognized.");
		}
	}
}
