using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.Cursors.Models;

public class TextEditorCursorModifier
{
    public TextEditorCursorModifier(TextEditorCursor cursor)
    {
        LineIndex = cursor.LineIndex;
        ColumnIndex = cursor.ColumnIndex;
        PreferredColumnIndex = cursor.PreferredColumnIndex;
        IsPrimaryCursor = cursor.IsPrimaryCursor;
        SelectionAnchorPositionIndex = cursor.Selection.AnchorPositionIndex;
        SelectionEndingPositionIndex = cursor.Selection.EndingPositionIndex;
        Key = cursor.Key;
    }

    public int LineIndex { get; set; }
    public int ColumnIndex {get; set; }
    public int PreferredColumnIndex {get; set; }
    public bool IsPrimaryCursor {get; set; }
    public int SelectionAnchorPositionIndex { get; set; }
    public int SelectionEndingPositionIndex { get; set; }
    public Key<TextEditorCursor> Key { get; set; }

    public TextEditorCursor ToCursor()
    {
        return new TextEditorCursor(
            LineIndex,
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