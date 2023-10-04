using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace Luthetus.TextEditor.Tests.Basics.TextEditor;

public class TextManipulationTests : LuthetusTextEditorTestingBase
{
    [Fact]
    public void INSERT_CHARACTER_BY_KEYBOARD_EVENT()
    {
        var startingText = TextEditorModel.GetAllText();

        Assert.Equal(string.Empty, startingText);
        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);

        var keyAKeyboardEventArg = new KeyboardEventArgs
        {
            Key = "a"
        };

        var keyboardEventAction = new TextEditorModelState.KeyboardEventAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            keyAKeyboardEventArg,
            CancellationToken.None);

        TextEditorService.Model.HandleKeyboardEvent(keyboardEventAction);

        var endingText = TextEditorModel.GetAllText();

        Assert.Equal("a", endingText);
        Assert.Equal((0, 1), TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void INSERT_CHARACTER_BY_DIRECT_INSERTION()
    {
        var startingText = TextEditorModel.GetAllText();

        Assert.Equal(string.Empty, startingText);
        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);

        var content = "a";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorService.Model.InsertText(insertTextAction);

        var endingText = TextEditorModel.GetAllText();

        var newlineCount = content.Count(x => x == '\n');

        var finalLineLength = content
            .Split('\n')
            .Last()
            .Length;

        Assert.Equal(content, endingText);
        Assert.Equal((newlineCount, finalLineLength), TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void INSERT_STRING_BY_DIRECT_INSERTION()
    {
        var startingText = TextEditorModel.GetAllText();

        Assert.Equal(string.Empty, startingText);
        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);

        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorService.Model.InsertText(insertTextAction);

        var endingText = TextEditorModel.GetAllText();

        var newlineCount = content.Count(x => x == '\n');

        var finalLineLength = content
            .Split('\n')
            .Last()
            .Length;

        Assert.Equal(content, endingText);
        Assert.Equal((newlineCount, finalLineLength), TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    /// <summary>
    /// 2023-21-03:
    ///     Description: When inserting "\r\n" the behavior is currently erroneous and this test will fail.
    ///     Expected: The insertion of "\r\n" is to result in "\r\n" being inserted. 
    ///     Actual: The insertion of "\r\n" results in "\n" being inserted. 
    /// </summary>
    [Fact]
    public void INSERT_CARRIAGE_RETURN_LINE_FEED()
    {
        var startingText = TextEditorModel.GetAllText();

        Assert.Equal(string.Empty, startingText);
        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);

        var content = "\r\n";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorService.Model.InsertText(insertTextAction);

        var endingText = TextEditorModel.GetAllText();

        Assert.Equal(content, endingText);
        Assert.Equal(2, TextEditorModel.RowCount);

        // "\r\n"
        {
            var carriageReturnLinefeedCounts = TextEditorModel.RowEndingKindCountsBag
                .Single(x =>
                    x.rowEndingKind == RowEndingKind.CarriageReturnLinefeed);

            Assert.Equal(1, carriageReturnLinefeedCounts.count);
        }

        // "\r"
        {
            var carriageReturnCounts = TextEditorModel.RowEndingKindCountsBag
                .Single(x =>
                    x.rowEndingKind == RowEndingKind.CarriageReturn);

            Assert.Equal(0, carriageReturnCounts.count);
        }

        // "\n"
        {
            var linefeedCounts = TextEditorModel.RowEndingKindCountsBag
                .Single(x =>
                    x.rowEndingKind == RowEndingKind.Linefeed);

            Assert.Equal(0, linefeedCounts.count);
        }
    }

    /// <summary>
    /// 2023-21-03:
    ///     Description: When inserting "\r" the behavior is currently erroneous and this test will fail.
    ///     Expected: The insertion of "\r" is to result in the CarriageReturn count to be 1 
    ///     Actual: The insertion of "\r" results in the CarriageReturn count to be 0
    /// 2023-21-03:
    ///     Description: When inserting "\r" the behavior is currently erroneous and this test will fail.
    ///     Expected: The insertion of "\r" is to result in the TextEditorModel.RowCount to be 2 
    ///     Actual: The insertion of "\r" results in the TextEditorModel.RowCount to be 1
    /// </summary>
    [Fact]
    public void INSERT_CARRIAGE_RETURN()
    {
        var startingText = TextEditorModel.GetAllText();

        Assert.Equal(string.Empty, startingText);
        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);

        var content = "\r";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorService.Model.InsertText(insertTextAction);

        var endingText = TextEditorModel.GetAllText();

        Assert.Equal(content, endingText);
        Assert.Equal(2, TextEditorModel.RowCount);

        // "\r\n"
        {
            var carriageReturnLinefeedCounts = TextEditorModel.RowEndingKindCountsBag
                .Single(x =>
                    x.rowEndingKind == RowEndingKind.CarriageReturnLinefeed);

            Assert.Equal(0, carriageReturnLinefeedCounts.count);
        }

        // "\r"
        {
            var carriageReturnCounts = TextEditorModel.RowEndingKindCountsBag
                .Single(x =>
                    x.rowEndingKind == RowEndingKind.CarriageReturn);

            Assert.Equal(1, carriageReturnCounts.count);
        }

        // "\n"
        {
            var linefeedCounts = TextEditorModel.RowEndingKindCountsBag
                .Single(x =>
                    x.rowEndingKind == RowEndingKind.Linefeed);

            Assert.Equal(0, linefeedCounts.count);
        }
    }

    /// <summary>
    /// 2023-21-03:
    ///     Description: When inserting "\n" the behavior is currently erroneous and this test will fail.
    ///     Expected: The insertion of "\n" is to result in the Linefeed count to be 1 
    ///     Actual: The insertion of "\r" results in the Linefeed count to be 0
    /// </summary>
    [Fact]
    public void INSERT_LINEFEED()
    {
        var startingText = TextEditorModel.GetAllText();

        Assert.Equal(string.Empty, startingText);
        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);

        var content = "\n";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorService.Model.InsertText(insertTextAction);

        var endingText = TextEditorModel.GetAllText();

        Assert.Equal(content, endingText);
        Assert.Equal(2, TextEditorModel.RowCount);

        // "\r\n"
        {
            var carriageReturnLinefeedCounts = TextEditorModel.RowEndingKindCountsBag
                .Single(x =>
                    x.rowEndingKind == RowEndingKind.CarriageReturnLinefeed);

            Assert.Equal(0, carriageReturnLinefeedCounts.count);
        }

        // "\r"
        {
            var carriageReturnCounts = TextEditorModel.RowEndingKindCountsBag
                .Single(x =>
                    x.rowEndingKind == RowEndingKind.CarriageReturn);

            Assert.Equal(0, carriageReturnCounts.count);
        }

        // "\n"
        {
            var linefeedCounts = TextEditorModel.RowEndingKindCountsBag
                .Single(x =>
                    x.rowEndingKind == RowEndingKind.Linefeed);

            Assert.Equal(1, linefeedCounts.count);
        }
    }

    /// <summary>
    /// Given the text:: "line1"
    ///     If a user makes an insertion request for specifically "\r"
    ///     and then a second insertion request for specifically "\n"
    ///     the code needs to handle this properly.
    /// </summary>
    [Fact]
    public void INSERT_CARRIAGE_RETURN_THEN_INSERT_LINE_FEED()
    {
        throw new NotImplementedException(
            "The behavior which occurs in this situation needs to be decided on." +
            " " +
            "Then a test needs made to ensure the functionality stays consistent.");
    }

    /// <summary>
    /// Given the text: "line 1\r\nline 2"
    ///     If a user requests to delete only the "\r\n" the code
    ///     needs to accurately update the row positions, among other things.
    /// </summary>
    [Fact]
    public void DELETE_CARRIAGE_RETURN_LINE_FEED()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Given the text: "line 1\rline 2"
    ///     If a user requests to delete only the "\r" the code
    ///     needs to accurately update the row positions, among other things.
    /// </summary>
    [Fact]
    public void DELETE_CARRIAGE_RETURN()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Given the text: "line 1\nline 2"
    ///     If a user requests to delete only the "\n" the code
    ///     needs to accurately update the row positions, among other things.
    /// </summary>
    [Fact]
    public void DELETE_LINE_FEED()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Given the text: "line 1\r\nline 2"
    ///     If a user requests to delete only the '\r' the code
    ///     needs to identify that the user requested to
    ///     delete a "\r\n" and then delete both characters.
    /// </summary>
    [Fact]
    public void DELETE_ONLY_THE_CARRIAGE_RETURN_OF_A_CARRIAGE_RETURN_LINE_FEED()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Given the text: "line 1\r\nline 2"
    ///     if a user requests to delete only the '\n' the code
    ///     needs to identify that the user requested to
    ///     delete a "\r\n" and then delete both characters.
    /// </summary>
    [Fact]
    public void DELETE_ONLY_THE_LINE_FEED_OF_A_CARRIAGE_RETURN_LINE_FEED()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void DELETE_SINGLE_CHARACTER()
    {
        var cursor = new TextEditorCursor((0, 0), true);

        var content = "A";

        TextEditorService.Model.InsertText(
            new TextEditorModelState.InsertTextAction(
                ResourceUri,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                content,
                CancellationToken.None));

        Assert.Equal(
            content,
            TextEditorModel.GetAllText());

        TextEditorService.Model.DeleteTextByRange(
            new TextEditorModelState.DeleteTextByRangeAction(
                ResourceUri,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                1,
                CancellationToken.None));

        Assert.Equal(
            string.Empty,
            TextEditorModel.GetAllText());
    }

    [Fact]
    public void DELETE_MANY_CHARACTERS()
    {
        var startingPositionIndex = 2;
        var count = 3;

        var cursor = new TextEditorCursor(
            (0, 0),
            true);

        var content = "Abcdefg";

        TextEditorService.Model.InsertText(
            new TextEditorModelState.InsertTextAction(
                ResourceUri,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                content,
                CancellationToken.None));

        Assert.Equal(
            content,
            TextEditorModel.GetAllText());

        cursor.IndexCoordinates = (0, startingPositionIndex);

        TextEditorService.Model.DeleteTextByRange(
            new TextEditorModelState.DeleteTextByRangeAction(
                ResourceUri,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                count,
                CancellationToken.None));

        Assert.Equal(
            content.Remove(startingPositionIndex, count),
            TextEditorModel.GetAllText());
    }
}