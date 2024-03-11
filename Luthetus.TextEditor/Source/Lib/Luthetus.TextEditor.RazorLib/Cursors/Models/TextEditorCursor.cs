using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.Cursors.Models;

public record TextEditorCursor(
    int RowIndex,
    int ColumnIndex,
    int PreferredColumnIndex,
    bool IsPrimaryCursor,
    TextEditorSelection Selection)
{
    public TextEditorCursor(bool isPrimaryCursor) 
        : this(0, 0, 0, isPrimaryCursor, TextEditorSelection.Empty)
    {
        
    }
    
    public TextEditorCursor(int rowIndex, int columnIndex, bool isPrimaryCursor) 
        : this(rowIndex, columnIndex, columnIndex, isPrimaryCursor, TextEditorSelection.Empty)
    {
        
    }

    public static readonly TextEditorCursor Empty = new(
        0,
        0,
        0,
        false,
        TextEditorSelection.Empty)
    {
        Key = Key<TextEditorCursor>.Empty
    };

    public Key<TextEditorCursor> Key { get; init; } = Key<TextEditorCursor>.NewKey();
}