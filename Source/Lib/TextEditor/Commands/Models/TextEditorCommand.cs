using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

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
    /// <summary>
    /// In order to avoid multiple invocations of <see cref="ITextEditorService.Post(string, TextEditorEdit)"/>
    /// one can continue on the current Post by invoking this TextEditorEdit directly.
    /// <br/><br/>
    /// If one invokes the <see cref="CommandNoType.CommandFunc"/> then a post will occur.
    /// <br/><br/>
    /// This is sphagetti code and needs to be changed. Vim's inner command logic
    /// has me writing this. But what I'm doing here is probably bad long term.
    /// One has to duplicate their <see cref="CommandNoType.CommandFunc"/> but
    /// without the Post invocation.
    /// <br/><br/>
    /// I think going down this path will reveal the true solution so I'm going to
    /// write out the bad code version for now. (2024-01-11)
    /// </summary>
    public Func<ICommandArgs, TextEditorEdit>? TextEditorEditFactory { get; set; }

    public static ApplicationException ThrowOtherTextEditKindIdentifierWasExpectedException(TextEditKind textEditKind)
    {
        throw new ApplicationException(
            $"{nameof(textEditKind)} was passed in as {TextEditKind.Other}" +
            $" therefore a {nameof(OtherTextEditKindIdentifier)} was expected" +
            $" however, the {nameof(OtherTextEditKindIdentifier)} passed in was null.");
    }
}