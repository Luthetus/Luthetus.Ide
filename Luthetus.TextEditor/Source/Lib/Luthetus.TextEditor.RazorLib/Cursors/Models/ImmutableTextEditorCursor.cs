namespace Luthetus.TextEditor.RazorLib.Cursors.Models;

public record ImmutableTextEditorCursor(
    int RowIndex,
    int ColumnIndex,
    int PreferredColumnIndex,
    ImmutableTextEditorSelection ImmutableSelection)
{
    public ImmutableTextEditorCursor(TextEditorCursor textEditorCursor)
        : this(
            textEditorCursor.IndexCoordinates.rowIndex,
            textEditorCursor.IndexCoordinates.columnIndex,
            textEditorCursor.PreferredColumnIndex,
            new ImmutableTextEditorSelection(textEditorCursor.Selection))
    {
    }
}