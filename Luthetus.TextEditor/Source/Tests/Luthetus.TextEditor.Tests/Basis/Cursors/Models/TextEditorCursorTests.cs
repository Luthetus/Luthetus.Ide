using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;

namespace Luthetus.TextEditor.Tests.Basis.Cursors.Models;

/// <summary>
/// <see cref="TextEditorCursor"/>
/// </summary>
public class TextEditorCursorTests
{
    /// <summary>
    /// <see cref="TextEditorCursor(int, int, int, bool, TextEditorSelection)"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorCursor.RowIndex"/>
    /// <see cref="TextEditorCursor.ColumnIndex"/>
    /// <see cref="TextEditorCursor.PreferredColumnIndex"/>
    /// <see cref="TextEditorCursor.IsPrimaryCursor"/>
    /// <see cref="TextEditorCursor.Selection"/>
    /// <see cref="TextEditorCursor.Key"/>
    /// </summary>
    [Fact]
    public void ConstructorA()
    {
        var rowIndex = 2;
        var columnIndex = 7;
        var preferredColumnIndex = 9;
        var isPrimaryCursor = true;
        var selection = new TextEditorSelection(1, 3);

        var cursor = new TextEditorCursor(
            rowIndex,
            columnIndex,
            preferredColumnIndex,
            isPrimaryCursor,
            selection);

        Assert.Equal(rowIndex, cursor.RowIndex);
        Assert.Equal(columnIndex, cursor.ColumnIndex);
        Assert.Equal(preferredColumnIndex, cursor.PreferredColumnIndex);
        Assert.Equal(isPrimaryCursor, cursor.IsPrimaryCursor);
        Assert.Equal(selection, cursor.Selection);

        // Assert default value is not Empty
        Assert.NotEqual(Key<TextEditorCursor>.Empty, cursor.Key);
    }

    /// <summary>
    /// <see cref="TextEditorCursor(bool)"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorCursor.RowIndex"/>
    /// <see cref="TextEditorCursor.ColumnIndex"/>
    /// <see cref="TextEditorCursor.PreferredColumnIndex"/>
    /// <see cref="TextEditorCursor.IsPrimaryCursor"/>
    /// <see cref="TextEditorCursor.Selection"/>
    /// <see cref="TextEditorCursor.Key"/>
    /// </summary>
    [Fact]
    public void ConstructorB()
    {
        var isPrimaryCursor = true;

        var cursor = new TextEditorCursor(isPrimaryCursor);

        Assert.Equal(0, cursor.RowIndex);
        Assert.Equal(0, cursor.ColumnIndex);
        Assert.Equal(0, cursor.PreferredColumnIndex);
        Assert.Equal(isPrimaryCursor, cursor.IsPrimaryCursor);
        Assert.Equal(TextEditorSelection.Empty, cursor.Selection);

        // Assert default value is not Empty
        Assert.NotEqual(Key<TextEditorCursor>.Empty, cursor.Key);
    }

    /// <summary>
    /// <see cref="TextEditorCursor(int, int, bool)"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorCursor.RowIndex"/>
    /// <see cref="TextEditorCursor.ColumnIndex"/>
    /// <see cref="TextEditorCursor.PreferredColumnIndex"/>
    /// <see cref="TextEditorCursor.IsPrimaryCursor"/>
    /// <see cref="TextEditorCursor.Selection"/>
    /// <see cref="TextEditorCursor.Key"/>
    /// </summary>
    [Fact]
    public void ConstructorC()
    {
        var rowIndex = 2;
        var columnIndex = 7;
        var isPrimaryCursor = true;

        var cursor = new TextEditorCursor(
            rowIndex,
            columnIndex,
            isPrimaryCursor);

        Assert.Equal(rowIndex, cursor.RowIndex);
        Assert.Equal(columnIndex, cursor.ColumnIndex);
        Assert.Equal(columnIndex, cursor.PreferredColumnIndex);
        Assert.Equal(isPrimaryCursor, cursor.IsPrimaryCursor);
        Assert.Equal(TextEditorSelection.Empty, cursor.Selection);

        // Assert default value is not Empty
        Assert.NotEqual(Key<TextEditorCursor>.Empty, cursor.Key);
    }

    /// <summary>
    /// <see cref="TextEditorCursor(TextEditorCursor)"/>
    /// </summary>
    [Fact]
    public void With()
    {
        var cursor = new TextEditorCursor(
            2,
            7,
            9,
            true,
            new TextEditorSelection(1, 3));

        // Assert 'with' new key
        {
            var outKey = Key<TextEditorCursor>.NewKey();
            
            // Assert the key value is not already set to ensure it changes.
            Assert.NotEqual(outKey, cursor.Key);

            var outCursor = cursor with
            {
                Key = outKey
            };

            Assert.Equal(outKey, outCursor.Key);
        }
    }
    
    /// <summary>
    /// <see cref="TextEditorCursor.Empty"/>
    /// </summary>
    [Fact]
    public void Empty()
    {
        Assert.Equal(0, TextEditorCursor.Empty.RowIndex);
        Assert.Equal(0, TextEditorCursor.Empty.ColumnIndex);
        Assert.Equal(0, TextEditorCursor.Empty.PreferredColumnIndex);
        Assert.False(TextEditorCursor.Empty.IsPrimaryCursor);
        Assert.Equal(TextEditorSelection.Empty, TextEditorCursor.Empty.Selection);

        // Assert default value IS Empty
        Assert.Equal(Key<TextEditorCursor>.Empty, TextEditorCursor.Empty.Key);
    }

    /// <summary>
    /// <see cref="TextEditorCursor.ShouldRevealCursor"/>
    /// </summary>
    [Fact]
    public void ShouldRevealCursor()
    {
        var cursor = new TextEditorCursor(
            2,
            7,
            9,
            true,
            new TextEditorSelection(1, 3));

        // Assert default value
        Assert.False(cursor.ShouldRevealCursor);

        // Assert when setter is given 'true' it is assigned
        cursor.ShouldRevealCursor = true;
        Assert.True(cursor.ShouldRevealCursor);

        // Assert when setter is given 'false' it is assigned
        cursor.ShouldRevealCursor = false;
        Assert.False(cursor.ShouldRevealCursor);
    }

    /// <summary>
    /// <see cref="TextEditorCursor.IsIntersecting"/>
    /// </summary>
    [Fact]
    public void IsIntersecting()
    {
        var cursor = new TextEditorCursor(
            2,
            7,
            9,
            true,
            new TextEditorSelection(1, 3));

        // Assert default value
        Assert.False(cursor.IsIntersecting);

        // Assert when setter is given 'true' it is assigned
        cursor.IsIntersecting = true;
        Assert.True(cursor.IsIntersecting);

        // Assert when setter is given 'false' it is assigned
        cursor.IsIntersecting = false;
        Assert.False(cursor.IsIntersecting);
    }
}