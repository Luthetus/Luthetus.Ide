using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

public partial record TextEditorState
{
	public record RegisterModelAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            TextEditorModel Model)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

    public record DisposeModelAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ResourceUri ResourceUri)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

    public record SetModelAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ITextEditorEditContext EditContext,
            TextEditorModelModifier ModelModifier)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

	public record RegisterViewModelAction(
        Key<TextEditorViewModel> ViewModelKey,
        ResourceUri ResourceUri,
        Category Category,
        ITextEditorService TextEditorService,
        IDispatcher Dispatcher,
        IDialogService DialogService,
        IJSRuntime JsRuntime);

    public record DisposeViewModelAction(Key<TextEditorViewModel> ViewModelKey);

    public record SetViewModelWithAction(
        	Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ITextEditorEditContext EditContext,
            Key<TextEditorViewModel> ViewModelKey,
            Func<TextEditorViewModel, TextEditorViewModel> WithFunc)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

	public record SetModelAndViewModelRangeAction(
            Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ITextEditorEditContext EditContext,
            List<TextEditorModelModifier> ModelModifierList,
			List<TextEditorViewModelModifier> ViewModelModifierList)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);
}
