using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace Luthetus.TextEditor.Tests.Basics.TextEditor;

public class MovementTests : TextEditorTestingBase
{
    [Fact]
    public void ARROW_LEFT_EMPTY()
    {
        var text = TextEditorModel.GetAllText();

        Assert.Equal(string.Empty, text);
        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);

        var arrowLeftKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT
        };

        TextEditorCursor.MoveCursor(
            arrowLeftKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void ARROW_LEFT_SHOULD_WRAP()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorHelper.TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 0;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (1, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var arrowLeftKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT
        };

        TextEditorCursor.MoveCursor(
            arrowLeftKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (0, content.IndexOf('\n')),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void ARROW_LEFT_SHOULD_NOT_WRAP()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorHelper.TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 1;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (1, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var arrowLeftKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT
        };

        TextEditorCursor.MoveCursor(
            arrowLeftKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (1, 0),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void ARROW_DOWN_SHOULD_CHANGE_ROW_INDEX_WITH_VALID_PREFERRED_INDEX()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorHelper.TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 3;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (0, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var arrowDownKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_DOWN
        };

        TextEditorCursor.MoveCursor(
            arrowDownKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (1, 3),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void ARROW_DOWN_SHOULD_CHANGE_ROW_INDEX_NOT_VALID_PREFERRED_INDEX()
    {
        var content = "Oh wait you forgot something!\nNo I didn't.";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorHelper.TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 17;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (0, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var arrowDownKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_DOWN
        };

        TextEditorCursor.MoveCursor(
            arrowDownKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (1, 12),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void ARROW_DOWN_SHOULD_NOT_CHANGE_ROW_INDEX()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorHelper.TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 3;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (1, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var arrowDownKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_DOWN
        };

        TextEditorCursor.MoveCursor(
            arrowDownKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (1, 3),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void ARROW_UP_SHOULD_CHANGE_ROW_INDEX_WITH_VALID_PREFERRED_INDEX()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorHelper.TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 3;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (1, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var arrowUpKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_UP
        };

        TextEditorCursor.MoveCursor(
            arrowUpKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (0, 3),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void ARROW_UP_SHOULD_CHANGE_ROW_INDEX_NOT_VALID_PREFERRED_INDEX()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorHelper.TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 21;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (1, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var arrowUpKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_UP
        };

        TextEditorCursor.MoveCursor(
            arrowUpKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (0, content.IndexOf('\n')),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void ARROW_UP_SHOULD_NOT_CHANGE_ROW_INDEX()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorHelper.TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 3;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (0, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var arrowUpKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_UP
        };

        TextEditorCursor.MoveCursor(
            arrowUpKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (0, 3),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void ARROW_RIGHT_EMPTY()
    {
        var text = TextEditorModel.GetAllText();

        Assert.Equal(string.Empty, text);
        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);

        var arrowRightKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT
        };

        TextEditorCursor.MoveCursor(
            arrowRightKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void ARROW_RIGHT_SHOULD_WRAP()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorHelper.TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = content.IndexOf('\n');

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (0, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var arrowRightKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT
        };

        TextEditorCursor.MoveCursor(
            arrowRightKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (1, 0),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void ARROW_RIGHT_SHOULD_NOT_WRAP()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorHelper.TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 1;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (0, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var arrowRightKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT
        };

        TextEditorCursor.MoveCursor(
            arrowRightKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (0, 2),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void HOME_SHOULD_CHANGE_COLUMN_INDEX()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorHelper.TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 3;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (0, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var homeKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.HOME
        };

        TextEditorCursor.MoveCursor(
            homeKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (0, 0),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void HOME_SHOULD_NOT_CHANGE_COLUMN_INDEX()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorHelper.TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 0;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (0, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var homeKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.HOME
        };

        TextEditorCursor.MoveCursor(
            homeKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (0, 0),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void END_SHOULD_CHANGE_COLUMN_INDEX()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorHelper.TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 3;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (0, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var endKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.END
        };

        TextEditorCursor.MoveCursor(
            endKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (0, content.IndexOf('\n')),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void END_SHOULD_NOT_CHANGE_COLUMN_INDEX()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorHelper.TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = content.IndexOf('\n');

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (0, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var endKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.END
        };

        TextEditorCursor.MoveCursor(
            endKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (0, content.IndexOf('\n')),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }
}