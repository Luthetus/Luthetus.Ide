using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;

namespace Luthetus.TextEditor.Tests.Basis.Commands.Models;

public class TextEditorCommandTests
{
    public TextEditorCommand(
            string displayName,
            string internalIdentifier,
            bool shouldBubble,
            bool shouldScrollCursorIntoView,
            TextEditKind textEditKind,
            string? otherTextEditKindIdentifier,
            Func<ICommandArgs, Task> doAsyncFunc)
        : base(displayName, internalIdentifier, shouldBubble, doAsyncFunc)
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