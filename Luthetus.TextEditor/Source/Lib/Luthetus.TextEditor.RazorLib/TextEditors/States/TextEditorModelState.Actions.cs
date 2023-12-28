using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

public partial class TextEditorModelState
{
    public record RegisterAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            TextEditorModel Model)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

    public record DisposeAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ResourceUri ResourceUri)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

    public record SetAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ITextEditorEditContext EditContext,
            TextEditorModelModifier ModelModifier)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);
}