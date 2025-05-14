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
		int before_LineIndex,
		int before_ColumnIndex,
		int before_PreferredColumnIndex,
		int before_SelectionAnchorPositionIndex,
		int before_SelectionEndingPositionIndex,
		int after_LineIndex,
		int after_ColumnIndex,
		int after_PreferredColumnIndex,
		int after_SelectionAnchorPositionIndex,
		int after_SelectionEndingPositionIndex,
		StringBuilder? editedTextBuilder)
	{
		EditKind = editKind;
		Tag = tag;
		BeforePositionIndex = beforePositionIndex;
		
		Before_LineIndex = before_LineIndex;
		Before_ColumnIndex = before_ColumnIndex;
		Before_PreferredColumnIndex = before_PreferredColumnIndex;
		Before_SelectionAnchorPositionIndex = before_SelectionAnchorPositionIndex;
		Before_SelectionEndingPositionIndex = before_SelectionEndingPositionIndex;
		
		After_LineIndex = after_LineIndex;
		After_ColumnIndex = after_ColumnIndex;
		After_PreferredColumnIndex = after_PreferredColumnIndex;
		After_SelectionAnchorPositionIndex = after_SelectionAnchorPositionIndex;
		After_SelectionEndingPositionIndex = after_SelectionEndingPositionIndex;
		
		EditedTextBuilder = editedTextBuilder;
	}

	public TextEditorEditKind EditKind { get; }
	public string Tag { get; } = string.Empty;
	
	public int BeforePositionIndex { get; }
	
	/// <summary>
	/// Whether the 'Cursor' position index is equal to the 'BeforePositionIndex' property on this type,
	/// I'm not sure. So I'm keeping the separate 'BeforePositionIndex' for now.
	/// </summary>
	public int Before_LineIndex { get; }
	public int Before_ColumnIndex { get; }
	public int Before_PreferredColumnIndex { get; }
	public int Before_SelectionAnchorPositionIndex { get; }
	public int Before_SelectionEndingPositionIndex { get; }
	
	public int After_LineIndex { get; }
	public int After_ColumnIndex { get; }
	public int After_PreferredColumnIndex { get; }
	public int After_SelectionAnchorPositionIndex { get; }
	public int After_SelectionEndingPositionIndex { get; }
	
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
			case TextEditorEditKind.DeleteSelection:
				throw new NotImplementedException("Cannot batch TextEditorEditKind.DeleteSelection");
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
					Before_LineIndex,
					Before_ColumnIndex,
					Before_PreferredColumnIndex,
					Before_SelectionAnchorPositionIndex,
					Before_SelectionEndingPositionIndex,
					After_LineIndex,
					After_ColumnIndex,
					After_PreferredColumnIndex,
					After_SelectionAnchorPositionIndex,
					After_SelectionEndingPositionIndex,
					EditedTextBuilder);
			case TextEditorEditKind.Backspace:
				return new TextEditorEdit(
					TextEditorEditKind.Insert,
					tag: string.Empty,
					BeforePositionIndex - EditedTextBuilder.Length,
					Before_LineIndex,
					Before_ColumnIndex,
					Before_PreferredColumnIndex,
					Before_SelectionAnchorPositionIndex,
					Before_SelectionEndingPositionIndex,
					After_LineIndex,
					After_ColumnIndex,
					After_PreferredColumnIndex,
					After_SelectionAnchorPositionIndex,
					After_SelectionEndingPositionIndex,
					EditedTextBuilder);
			case TextEditorEditKind.Delete:
				return new TextEditorEdit(
					TextEditorEditKind.Insert,
					tag: string.Empty,
					BeforePositionIndex,
					Before_LineIndex,
					Before_ColumnIndex,
					Before_PreferredColumnIndex,
					Before_SelectionAnchorPositionIndex,
					Before_SelectionEndingPositionIndex,
					After_LineIndex,
					After_ColumnIndex,
					After_PreferredColumnIndex,
					After_SelectionAnchorPositionIndex,
					After_SelectionEndingPositionIndex,
					EditedTextBuilder);
			case TextEditorEditKind.DeleteSelection:
				return new TextEditorEdit(
					TextEditorEditKind.Insert,
					tag: string.Empty,
					BeforePositionIndex,
					Before_LineIndex,
					Before_ColumnIndex,
					Before_PreferredColumnIndex,
					Before_SelectionAnchorPositionIndex,
					Before_SelectionEndingPositionIndex,
					After_LineIndex,
					After_ColumnIndex,
					After_PreferredColumnIndex,
					After_SelectionAnchorPositionIndex,
					After_SelectionEndingPositionIndex,
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
