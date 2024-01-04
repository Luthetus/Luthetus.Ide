using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Commands.Models;

public class TextEditorCommand : CommandWithType<TextEditorCommandArgs>
{
    public TextEditorCommand(
            string displayName,
            string internalIdentifier,
            bool shouldBubble,
            bool shouldScrollCursorIntoView,
            TextEditKind textEditKind,
            string? otherTextEditKindIdentifier,
            Func<ICommandArgs, Task> commandFunc)
        : base(displayName, internalIdentifier, shouldBubble, commandFunc)
    {
        if (textEditKind == TextEditKind.Other && otherTextEditKindIdentifier is null)
            ThrowOtherTextEditKindIdentifierWasExpectedException(textEditKind);

        ShouldScrollCursorIntoView = shouldScrollCursorIntoView;
        TextEditKind = textEditKind;
        OtherTextEditKindIdentifier = otherTextEditKindIdentifier;
    }

    public bool ShouldScrollCursorIntoView { get; }
    public TextEditKind TextEditKind { get; }
    public string? OtherTextEditKindIdentifier { get; }

    public delegate Task ModificationTask(
        TextEditorCommandArgs commandArgs,
        TextEditorModel model,
        TextEditorViewModel viewModel,
        TextEditorCursorModifierBag refreshCursorsRequest,
        TextEditorCursorModifier primaryCursor);

    public static ApplicationException ThrowOtherTextEditKindIdentifierWasExpectedException(TextEditKind textEditKind)
    {
        throw new ApplicationException(
            $"{nameof(textEditKind)} was passed in as {TextEditKind.Other}" +
            $" therefore a {nameof(OtherTextEditKindIdentifier)} was expected" +
            $" however, the {nameof(OtherTextEditKindIdentifier)} passed in was null.");
    }
}