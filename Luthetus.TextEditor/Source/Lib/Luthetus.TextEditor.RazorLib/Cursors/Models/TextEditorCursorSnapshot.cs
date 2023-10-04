using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Cursors.Models;

public class TextEditorCursorSnapshot
{
    public TextEditorCursorSnapshot(TextEditorCursor userCursor)
        : this(new ImmutableTextEditorCursor(userCursor), userCursor)
    {
    }

    public TextEditorCursorSnapshot(ImmutableTextEditorCursor immutableCursor, TextEditorCursor userCursor)
    {
        ImmutableCursor = immutableCursor;
        UserCursor = userCursor;
    }

    public ImmutableTextEditorCursor ImmutableCursor { get; }
    public TextEditorCursor UserCursor { get; }

    public static ImmutableArray<TextEditorCursorSnapshot> TakeSnapshots(params TextEditorCursor[] cursorBag)
    {
        return cursorBag.Select(c => new TextEditorCursorSnapshot(c)).ToImmutableArray();
    }
}