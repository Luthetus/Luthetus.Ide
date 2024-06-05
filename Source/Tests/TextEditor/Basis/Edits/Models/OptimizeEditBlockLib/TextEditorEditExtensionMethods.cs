using Luthetus.TextEditor.RazorLib.Exceptions;

namespace Luthetus.TextEditor.Tests.Basis.Edits.Models.OptimizeEditBlockLib;

public static class TextEditorEditExtensionMethods
{
	public static ITextEditorEdit ToUndo(this ITextEditorEdit edit)
	{
		switch (edit.EditKind)
		{
			case TextEditorEditKind.Insert:
				var editInsert = (TextEditorEditInsert)edit;
				return new TextEditorEditDelete(editInsert.PositionIndex, editInsert.Content.Length);
			case TextEditorEditKind.InsertBatch:
				var editInsertBatch = (TextEditorEditInsertBatch)edit;
				return new TextEditorEditDelete(editInsertBatch.PositionIndex, editInsertBatch.ContentBuilder.Length);
			case TextEditorEditKind.Backspace:
				var editBackspace = (TextEditorEditBackspace)edit;
				return new TextEditorEditInsert(editBackspace.PositionIndex - editBackspace.TextDeleted.Length, editBackspace.TextDeleted);
			case TextEditorEditKind.BackspaceBatch:
				var editBackspaceBatch = (TextEditorEditBackspaceBatch)edit;
				return new TextEditorEditInsert(editBackspaceBatch.PositionIndex, editBackspaceBatch.TextDeletedBuilder.ToString());
			case TextEditorEditKind.Delete:
				var editDelete = (TextEditorEditDelete)edit;
				return new TextEditorEditInsert(editDelete.PositionIndex, editDelete.TextDeleted);
			case TextEditorEditKind.DeleteBatch:
				var editDeleteBatch = (TextEditorEditDeleteBatch)edit;
				return new TextEditorEditInsert(editDeleteBatch.PositionIndex, editDeleteBatch.TextDeletedBuilder.ToString());
			case TextEditorEditKind.Constructor:
				throw new LuthetusTextEditorException("The {nameof(TextEditorEditKind)}: {edit.EditKind}, cannot be un-done. This edit represents the initial state.");
			case TextEditorEditKind.Other: 
				throw new NotImplementedException("TODO: Handle {nameof(TextEditorEditKind)}.{edit.EditKind}");
			default:
				throw new NotImplementedException($"The {nameof(TextEditorEditKind)}: {edit.EditKind} was not recognized.");
		}
	}
}
