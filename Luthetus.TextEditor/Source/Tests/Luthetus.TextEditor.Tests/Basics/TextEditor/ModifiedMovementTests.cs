using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace Luthetus.TextEditor.Tests.Basics.TextEditor;

public class ModifiedMovementTests : LuthetusTextEditorTestingBase
{
    [Fact]
    public void CONTROL_ARROW_LEFT_EMPTY()
    {
        var text = TextEditorModel.GetAllText();

        Assert.Equal(string.Empty, text);
        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);

        var controlArrowLeftKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
            CtrlKey = true
        };

        TextEditorCursor.MoveCursor(
            controlArrowLeftKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void CONTROL_ARROW_LEFT_SHOULD_WRAP()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 0;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (1, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var controlArrowLeftKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
            CtrlKey = true
        };

        TextEditorCursor.MoveCursor(
            controlArrowLeftKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (0, content.IndexOf('\n')),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void CONTROL_ARROW_LEFT_SHOULD_NOT_WRAP()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 2;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (1, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var controlArrowLeftKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
            CtrlKey = true
        };

        TextEditorCursor.MoveCursor(
            controlArrowLeftKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (1, 0),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void CONTROL_ARROW_RIGHT_EMPTY()
    {
        var text = TextEditorModel.GetAllText();

        Assert.Equal(string.Empty, text);
        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);

        var controlArrowRightKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
            CtrlKey = true
        };

        TextEditorCursor.MoveCursor(
            controlArrowRightKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void CONTROL_ARROW_RIGHT_SHOULD_WRAP()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = content.IndexOf('\n');

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (0, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var controlArrowRightKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
            CtrlKey = true
        };

        TextEditorCursor.MoveCursor(
            controlArrowRightKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (1, 0),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void CONTROL_ARROW_RIGHT_SHOULD_NOT_WRAP()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 1;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (0, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var controlArrowRightKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
            CtrlKey = true
        };

        TextEditorCursor.MoveCursor(
            controlArrowRightKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (0, 3),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void CONTROL_HOME_SHOULD_CHANGE_ROW_INDEX_AND_COLUMN_INDEX()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 3;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (1, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var controlHomeKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.HOME,
            CtrlKey = true
        };

        TextEditorCursor.MoveCursor(
            controlHomeKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (0, 0),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void CONTROL_HOME_SHOULD_CHANGE_COLUMN_INDEX_BUT_NOT_ROW_INDEX()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 3;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (0, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var controlHomeKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.HOME,
            CtrlKey = true
        };

        TextEditorCursor.MoveCursor(
            controlHomeKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (0, 0),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void CONTROL_HOME_SHOULD_DO_NOTHING()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 0;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (0, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var controlHomeKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.HOME,
            CtrlKey = true
        };

        TextEditorCursor.MoveCursor(
            controlHomeKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (0, 0),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void CONTROL_END_SHOULD_CHANGE_ROW_INDEX_AND_COLUMN_INDEX()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 3;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (0, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var controlEndKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.END,
            CtrlKey = true
        };

        TextEditorCursor.MoveCursor(
            controlEndKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (1, 27),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void CONTROL_END_SHOULD_CHANGE_COLUMN_INDEX_BUT_NOT_ROW_INDEX()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 3;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (1, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var controlEndKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.END,
            CtrlKey = true
        };

        TextEditorCursor.MoveCursor(
            controlEndKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (1, 27),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void CONTROL_END_SHOULD_DO_NOTHING()
    {
        var content = "See you later!\nOh wait I forgot something!";

        var insertTextAction = new TextEditorModelState.InsertTextAction(
            ResourceUri,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);

        TextEditorService.Model.InsertText(insertTextAction);

        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);

        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 27;

        TextEditorViewModel.PrimaryCursor.IndexCoordinates =
            (1, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);

        var controlEndKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.END,
            CtrlKey = true
        };

        TextEditorCursor.MoveCursor(
            controlEndKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (1, 27),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }
}