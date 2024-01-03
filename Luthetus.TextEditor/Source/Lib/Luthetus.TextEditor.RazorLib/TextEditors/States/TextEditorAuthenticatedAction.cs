using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

/// <summary>
/// Any actions which modify state should inhert from <see cref="TextEditorAuthenticatedAction"/>.<br/>
/// <br/>
/// Doing this permits state modifications to be concurrency safe. They will be enqueued using
/// <see cref="ITextEditorService.EnqueueEdit(Func{ITextEditorEditContext, Task})"/>.<br/>
/// <br/>
/// When it is a specific edit's turn to perform their modification, all state will be provided
/// by the <see cref="ITextEditorService"/>.<br/>
/// <br/>
/// Therefore no state modification can be done without the <see cref="TextEditorService.AuthenticatedActionKey"/>.<br/>
/// <br/>
/// The <see cref="TextEditorService.AuthenticatedActionKey"/> is public, so one could subvert this.
/// But, it shouldn't be subverted, the key is public in order to provide clarity in how
/// the "authentication" is being done.<br/>
/// <br/>
/// TODO: Learn more about <see cref="SynchronizationContext"/>
/// </summary>
public record TextEditorAuthenticatedAction
{
    public TextEditorAuthenticatedAction(Key<TextEditorAuthenticatedAction> authenticatedActionKey)
    {
        if (authenticatedActionKey != TextEditorService.AuthenticatedActionKey)
            throw new ApplicationException($"Only edits made via the {nameof(ITextEditorService)}.{nameof(ITextEditorService.Post)}(...) method may modify state.");
        AuthenticatedActionKey = authenticatedActionKey;
    }

    public Key<TextEditorAuthenticatedAction> AuthenticatedActionKey { get; }
}