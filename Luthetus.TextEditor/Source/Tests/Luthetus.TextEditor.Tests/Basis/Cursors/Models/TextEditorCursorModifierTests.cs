using Xunit;
using Luthetus.TextEditor.RazorLib.Cursors.Models;

namespace Luthetus.TextEditor.Tests.Basis.Cursors.Models;

/// <summary>
/// <see cref="TextEditorCursorModifier"/>
/// <br/>----<br/>
/// <see cref="TextEditorCursorModifier.RowIndex"/>
/// <see cref="TextEditorCursorModifier.ColumnIndex"/>
/// <see cref="TextEditorCursorModifier.PreferredColumnIndex"/>
/// <see cref="TextEditorCursorModifier.IsPrimaryCursor"/>
/// <see cref="TextEditorCursorModifier.SelectionAnchorPositionIndex"/>
/// <see cref="TextEditorCursorModifier.SelectionEndingPositionIndex"/>
/// <see cref="TextEditorCursorModifier.Key"/>
/// <see cref="TextEditorCursorModifier.ShouldRevealCursor"/>
/// <see cref="TextEditorCursorModifier.IsIntersecting"/>
/// <see cref="TextEditorCursorModifier.ToCursor()"/>
/// </summary>
public class TextEditorCursorModifierTests
{
    /// <summary>
    /// <see cref="TextEditorCursorModifier(TextEditorCursor)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var cursor = new TextEditorCursor(
            2,
            7,
            9,
            true,
            new TextEditorSelection(1, 3));

        var cursorModifier = new TextEditorCursorModifier(cursor);

        Assert.Equal(cursor.RowIndex, cursorModifier.RowIndex);
        Assert.Equal(cursor.ColumnIndex, cursorModifier.ColumnIndex);
        Assert.Equal(cursor.PreferredColumnIndex, cursorModifier.PreferredColumnIndex);
        Assert.Equal(cursor.IsPrimaryCursor, cursorModifier.IsPrimaryCursor);
        Assert.Equal(cursor.Selection.AnchorPositionIndex, cursorModifier.SelectionAnchorPositionIndex);
        Assert.Equal(cursor.Selection.EndingPositionIndex, cursorModifier.SelectionEndingPositionIndex);
        Assert.Equal(cursor.Key, cursorModifier.Key);
        Assert.Equal(cursor.ShouldRevealCursor, cursorModifier.ShouldRevealCursor);
        Assert.Equal(cursor.IsIntersecting, cursorModifier.IsIntersecting);

        // Assert state mutations
        {
            // RowIndex
            {
                var inRowIndex = cursorModifier.RowIndex;
                cursorModifier.RowIndex++;
                Assert.Equal(inRowIndex + 1, cursorModifier.RowIndex);
            }

            // ColumnIndex
            {
                var inColumnIndex = cursorModifier.ColumnIndex;
                cursorModifier.ColumnIndex++;
                Assert.Equal(inColumnIndex + 1, cursorModifier.ColumnIndex);
            }

            // PreferredColumnIndex
            {
                var inPreferredColumnIndex = cursorModifier.PreferredColumnIndex;
                cursorModifier.PreferredColumnIndex++;
                Assert.Equal(inPreferredColumnIndex + 1, cursorModifier.PreferredColumnIndex);
            }

            // IsPrimaryCursor
            {
                var inIsPrimaryCursor = cursorModifier.IsPrimaryCursor;
                cursorModifier.IsPrimaryCursor = !cursorModifier.IsPrimaryCursor;
                Assert.Equal(!inIsPrimaryCursor, cursorModifier.IsPrimaryCursor);
            }

            // SelectionAnchorPositionIndex
            {
                var inSelectionAnchorPositionIndex = cursorModifier.SelectionAnchorPositionIndex;
                cursorModifier.SelectionAnchorPositionIndex++;
                Assert.Equal(inSelectionAnchorPositionIndex + 1, cursorModifier.SelectionAnchorPositionIndex);
            }

            // SelectionEndingPositionIndex
            {
                var inSelectionEndingPositionIndex = cursorModifier.SelectionEndingPositionIndex;
                cursorModifier.SelectionEndingPositionIndex++;
                Assert.Equal(inSelectionEndingPositionIndex + 1, cursorModifier.SelectionEndingPositionIndex);
            }

            // ShouldRevealCursor
            {
                var inShouldRevealCursor = cursorModifier.ShouldRevealCursor;
                cursorModifier.ShouldRevealCursor = !cursorModifier.ShouldRevealCursor;
                Assert.Equal(!inShouldRevealCursor, cursorModifier.ShouldRevealCursor);
            }

            // IsIntersecting
            {
                var inIsIntersecting = cursorModifier.IsIntersecting;
                cursorModifier.IsIntersecting = !cursorModifier.IsIntersecting;
                Assert.Equal(!inIsIntersecting, cursorModifier.IsIntersecting);
            }
        }

        var outCursor = cursorModifier.ToCursor();

        Assert.Equal(cursorModifier.RowIndex, outCursor.RowIndex);
        Assert.Equal(cursorModifier.ColumnIndex, outCursor.ColumnIndex);
        Assert.Equal(cursorModifier.PreferredColumnIndex, outCursor.PreferredColumnIndex);
        Assert.Equal(cursorModifier.IsPrimaryCursor, outCursor.IsPrimaryCursor);
        Assert.Equal(cursorModifier.SelectionAnchorPositionIndex, outCursor.Selection.AnchorPositionIndex);
        Assert.Equal(cursorModifier.SelectionEndingPositionIndex, outCursor.Selection.EndingPositionIndex);
        Assert.Equal(cursorModifier.Key, outCursor.Key);
        Assert.Equal(cursorModifier.ShouldRevealCursor, outCursor.ShouldRevealCursor);
        Assert.Equal(cursorModifier.IsIntersecting, outCursor.IsIntersecting);
    }
}
