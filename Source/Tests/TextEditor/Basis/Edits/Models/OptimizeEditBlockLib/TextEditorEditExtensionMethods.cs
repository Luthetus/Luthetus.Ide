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
			case TextEditorEditKind.Backspace: 
				return new TextEditorEditInsert();
			case TextEditorEditKind.Delete: 
				return new TextEditorEditInsert();
			case TextEditorEditKind.Other: 
				throw new NotImplementedException("TODO: Handle {nameof(TextEditorEditKind)}.{edit.EditKind}");
			default:
				throw new NotImplementedException($"The {nameof(TextEditorEditKind)}: {edit.EditKind} was not recognized.");
		}
	}
}
