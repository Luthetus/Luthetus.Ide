using Luthetus.TextEditor.RazorLib.Exceptions;

namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public static class TextEditorEditExtensionMethods
{
	public static ITextEditorEdit ToUndo(this ITextEditorEdit edit)
	{
		switch (edit.EditKind)
		{
			case TextEditorEditKind.Insert:
				var editInsertBatch = (TextEditorEditInsert)edit;
				return new TextEditorEditDelete(
					editInsertBatch.PositionIndex,
					editInsertBatch.ContentBuilder.Length,
					editInsertBatch.ContentBuilder);
			case TextEditorEditKind.Backspace:
				var editBackspaceBatch = (TextEditorEditBackspace)edit;
				return new TextEditorEditInsert(
					editBackspaceBatch.PositionIndex - editBackspaceBatch.TextRemovedBuilder.Length,
					editBackspaceBatch.TextRemovedBuilder);
			case TextEditorEditKind.Delete:
				var editDeleteBatch = (TextEditorEditDelete)edit;
				return new TextEditorEditInsert(
					editDeleteBatch.PositionIndex,
					editDeleteBatch.TextRemovedBuilder);
			case TextEditorEditKind.Constructor:
				throw new LuthetusTextEditorException($"The {nameof(TextEditorEditKind)}: {edit.EditKind}, cannot be un-done. This edit represents the initial state.");
			case TextEditorEditKind.Other: 
				return edit;
			default:
				throw new NotImplementedException($"The {nameof(TextEditorEditKind)}: {edit.EditKind} was not recognized.");
		}
	}
}
