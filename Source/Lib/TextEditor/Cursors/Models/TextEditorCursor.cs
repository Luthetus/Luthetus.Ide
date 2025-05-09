using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.Cursors.Models;

public struct TextEditorCursor
{
	public TextEditorCursor(
		int lineIndex,
	    int columnIndex,
	    int preferredColumnIndex,
	    bool isPrimaryCursor,
	    TextEditorSelection selection)
	{
		LineIndex = lineIndex;
	    ColumnIndex = columnIndex;
	    PreferredColumnIndex = preferredColumnIndex;
	    IsPrimaryCursor = isPrimaryCursor;
	    Selection = selection;
	}

    public TextEditorCursor(bool isPrimaryCursor) 
        : this(0, 0, 0, isPrimaryCursor, TextEditorSelection.Empty)
    {
        
    }
    
    public TextEditorCursor(int lineIndex, int columnIndex, bool isPrimaryCursor) 
        : this(lineIndex, columnIndex, columnIndex, isPrimaryCursor, TextEditorSelection.Empty)
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
    
    public int LineIndex;
    public int ColumnIndex;
    public int PreferredColumnIndex;
    public bool IsPrimaryCursor;
    public TextEditorSelection Selection;
}