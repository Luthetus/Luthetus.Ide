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

    public int LineIndex;
    public int ColumnIndex;
    public int PreferredColumnIndex;
    public bool IsPrimaryCursor;
    public int SelectionAnchorPositionIndex;
    public int SelectionEndingPositionIndex;
    public Key<TextEditorCursor> Key;

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