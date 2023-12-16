using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.Cursors.Models;

public record TextEditorCursor(
    int RowIndex,
    int ColumnIndex,
    int PreferredColumnIndex,
    bool IsPrimaryCursor,
    TextEditorSelection Selection)
{
    public static readonly TextEditorCursor Empty = new(
        0,
        0,
        0,
        false,
        TextEditorSelection.Empty);

    public Key<TextEditorCursor> Key { get; init; }
    public bool ShouldRevealCursor { get; set; }
    /// <summary>
    /// Relates to whether the cursor is within the viewable area of the Text Editor on the UI
    /// </summary>
    public bool IsIntersecting { get; set; }
}