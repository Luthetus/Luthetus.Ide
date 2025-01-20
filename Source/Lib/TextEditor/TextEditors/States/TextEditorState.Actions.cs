using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
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
        
    public record RegisterViewModelExistingAction(TextEditorViewModel ViewModel);

    public record DisposeViewModelAction(Key<TextEditorViewModel> ViewModelKey);

    public record SetViewModelWithAction(
        	Key<TextEditorAuthenticatedAction> AuthenticatedActionKey,
            ITextEditorEditContext EditContext,
            Key<TextEditorViewModel> ViewModelKey,
            Func<TextEditorViewModel, TextEditorViewModel> WithFunc)
        : TextEditorAuthenticatedAction(AuthenticatedActionKey);

	public struct SetModelAndViewModelRangeAction
	{
		public SetModelAndViewModelRangeAction(
			Key<TextEditorAuthenticatedAction> authenticatedActionKey,
			ITextEditorEditContext editContext,
			Dictionary<ResourceUri, TextEditorModelModifier?>? modelModifierList,
			Dictionary<Key<TextEditorViewModel>, TextEditorViewModelModifier?>? viewModelModifierList)
		{
			if (authenticatedActionKey != TextEditorService.AuthenticatedActionKey)
	            throw new LuthetusTextEditorException($"Only edits made via the {nameof(ITextEditorService)}.{nameof(ITextEditorService.Post)}(...) method may modify state.");
		
			AuthenticatedActionKey = authenticatedActionKey;
			EditContext = editContext;
			ModelModifierList = modelModifierList;
			ViewModelModifierList = viewModelModifierList;
		}
		
		/// <summary>
		/// It used to be the case that the class 'TextEditorAuthenticatedAction' was being
		/// inherited, in order to not repeat the assertion that this property ==
		/// <see cref="TextEditorService.AuthenticatedActionKey"/>.
		///
		/// But, this action Type in particular is a very hot path, so
		/// some experimental changes are being made to see the impact on performance.
		///
		/// The idea is, with this firing 20+ times every second during a continued mousemovement event,
		/// that perhaps making this into a struct could avoid some garbage collection overhead.
		///
		/// Not only is it being done for the performance sake, but this Type also lends itself to a
		/// struct, since it is scoped to the function that invoked the 'Dispatcher.Dispatch(object action)'.
		/// </summary>
		public Key<TextEditorAuthenticatedAction> AuthenticatedActionKey { get; }
		
        public ITextEditorEditContext EditContext { get; }
        public Dictionary<ResourceUri, TextEditorModelModifier?>? ModelModifierList { get; }
		public Dictionary<Key<TextEditorViewModel>, TextEditorViewModelModifier?>? ViewModelModifierList { get; }
	}
}
