using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.Cursors.Models;

public class TextEditorCursorModifier
{
    public TextEditorCursorModifier(TextEditorCursor cursor)
    {
        RowIndex = cursor.RowIndex;
        ColumnIndex = cursor.ColumnIndex;
        PreferredColumnIndex = cursor.PreferredColumnIndex;
        IsPrimaryCursor = cursor.IsPrimaryCursor;
        SelectionAnchorPositionIndex = cursor.Selection.AnchorPositionIndex;
        SelectionEndingPositionIndex = cursor.Selection.EndingPositionIndex;
        Key = cursor.Key;
    }

    public int RowIndex { get; set; }
    public int ColumnIndex {get; set; }
    public int PreferredColumnIndex {get; set; }
    public bool IsPrimaryCursor {get; set; }
    public int? SelectionAnchorPositionIndex { get; set; }
    public int SelectionEndingPositionIndex { get; set; }
    public Key<TextEditorCursor> Key { get; init; }
    public bool ShouldRevealCursor { get; set; }
    /// <summary>
    /// Relates to whether the cursor is within the viewable area of the Text Editor on the UI
    /// </summary>
    public bool IsIntersecting { get; set; }

    public TextEditorCursor ToCursor()
    {
        return new TextEditorCursor(
            RowIndex,
            ColumnIndex,
            PreferredColumnIndex,
            IsPrimaryCursor,
            new TextEditorSelection(SelectionAnchorPositionIndex, SelectionEndingPositionIndex))
        {
            Key = Key,
        };
    }

    public void SetColumnIndexAndPreferred(int columnIndex)
    {
        ColumnIndex = columnIndex;
        PreferredColumnIndex = columnIndex;
    }
}