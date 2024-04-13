using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.Common.RazorLib.Commands.Models;

namespace Luthetus.TextEditor.Tests.Basis.Commands.Models;

/// <summary>
/// <see cref="TextEditorCommand"/>
/// </summary>
public class TextEditorCommandTests
{
    /// <summary>
    /// <see cref="TextEditorCommand(string, string, bool, bool, TextEditKind, string?, Func{ICommandArgs, Task})"/>
    /// <br/>----<br/>
	/// <see cref="TextEditorCommand.ShouldScrollCursorIntoView"/>
    /// <see cref="TextEditorCommand.TextEditKind"/>
    /// <see cref="TextEditorCommand.OtherTextEditKindIdentifier"/>
	/// <see cref="TextEditorCommand.ThrowOtherTextEditKindIdentifierWasExpectedException(TextEditKind)"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		string displayName = "Paste";
        string internalIdentifier = "defaults_paste";
        bool shouldBubble = false;
        bool shouldScrollCursorIntoView = true;
        TextEditKind textEditKind = TextEditKind.Other;
        string? otherTextEditKindIdentifier = "defaults_paste";
        Func<ICommandArgs, Task> commandFunc = commandArgs => Task.CompletedTask;

        var command = new TextEditorCommand(
            displayName,
            internalIdentifier,
            shouldBubble,
            shouldScrollCursorIntoView,
            textEditKind,
            otherTextEditKindIdentifier,
            commandFunc);

		Assert.Equal(displayName, command.DisplayName);
		Assert.Equal(internalIdentifier, command.InternalIdentifier);
		Assert.Equal(shouldBubble, command.ShouldBubble);
        Assert.Equal(shouldScrollCursorIntoView, command.ShouldScrollCursorIntoView);
        Assert.Equal(textEditKind, command.TextEditKind);
        Assert.Equal(otherTextEditKindIdentifier, command.OtherTextEditKindIdentifier);
        Assert.Equal(commandFunc, command.CommandFunc);

        // ThrowOtherTextEditKindIdentifierWasExpectedException(TextEditKind)
        Assert.Throws<ApplicationException>(() =>
        {
            var command = new TextEditorCommand(
                displayName,
                internalIdentifier,
                shouldBubble,
                shouldScrollCursorIntoView,
                textEditKind,
                null,
                commandFunc);
        });
	}
}