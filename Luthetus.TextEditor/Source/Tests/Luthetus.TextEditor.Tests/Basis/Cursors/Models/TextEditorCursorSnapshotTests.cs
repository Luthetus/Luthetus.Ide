using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.Cursors.Models;

public class TextEditorCursorSnapshotTests
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