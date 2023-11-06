using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;

namespace Luthetus.TextEditor.RazorLib.Commands.Models;

public class TextEditorCommand : CommandWithType<TextEditorCommandArgs>
{
    public TextEditorCommand(
            Func<ICommandArgs, Task> doAsyncFunc,
            bool shouldScrollCursorIntoView,
            string displayName,
            string internalIdentifier,
            TextEditKind textEditKind = TextEditKind.None,
            string? otherTextEditKindIdentifier = null)
        : base(doAsyncFunc, displayName, internalIdentifier, false)
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

    public static ApplicationException ThrowOtherTextEditKindIdentifierWasExpectedException(TextEditKind textEditKind)
    {
        throw new ApplicationException(
            $"{nameof(textEditKind)} was passed in as {TextEditKind.Other}" +
            $" therefore a {nameof(OtherTextEditKindIdentifier)} was expected" +
            $" however, the {nameof(OtherTextEditKindIdentifier)} passed in was null.");
    }
}