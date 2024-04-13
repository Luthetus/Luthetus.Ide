using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.JSInterop;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

public partial class TextEditorViewModelState
{
    public record RegisterAction(
        Key<TextEditorViewModel> ViewModelKey,
        ResourceUri ResourceUri,
        Category Category,
        ITextEditorService TextEditorService,
        IDispatcher Dispatcher,
        IDialogService DialogService,
        IJSRuntime JsRuntime);

    public record DisposeAction(Key<TextEditorViewModel> ViewModelKey);

    public record SetViewModelWithAction(
        Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            IEditContext EditContext,
            Key<TextEditorViewModel> ViewModelKey,
            Func<TextEditorViewModel, TextEditorViewModel> WithFunc)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);
}