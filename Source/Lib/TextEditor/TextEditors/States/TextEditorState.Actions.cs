using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

/// <summary>Do not use any of these actions directly.</summary>
public partial record TextEditorState
{
	public struct RegisterModelAction
	{
		public RegisterModelAction(TextEditorModel model)
		{
			Model = model;
		}
	
		public TextEditorModel Model { get; }
	}

	public struct DisposeModelAction
	{
		public DisposeModelAction(ResourceUri resourceUri)
		{
			ResourceUri = resourceUri;
		}
		
		public ResourceUri ResourceUri { get; }
	}

    public struct SetModelAction
    {
    	public SetModelAction(ITextEditorEditContext editContext, TextEditorModelModifier modelModifier)
    	{
    		EditContext = editContext;
    		ModelModifier = modelModifier;
    	}
    	
        public ITextEditorEditContext EditContext { get; }
        public TextEditorModelModifier ModelModifier { get; }
    }

	public struct RegisterViewModelAction
	{
		public RegisterViewModelAction(
			Key<TextEditorViewModel> viewModelKey,
	        ResourceUri resourceUri,
	        Category category,
	        ITextEditorService textEditorService,
	        IDispatcher dispatcher,
	        IDialogService dialogService,
	        IJSRuntime jsRuntime)
		{
			ViewModelKey = viewModelKey;
	        ResourceUri = resourceUri;
	        Category = category;
	        TextEditorService = textEditorService;
	        Dispatcher = dispatcher;
	        DialogService = dialogService;
	        DialogService = dialogService;
		}
		
        public Key<TextEditorViewModel> ViewModelKey { get; }
        public ResourceUri ResourceUri { get; }
        public Category Category { get; }
        public ITextEditorService TextEditorService { get; }
        public IDispatcher Dispatcher { get; }
        public IDialogService DialogService { get; }
        public IJSRuntime JsRuntime { get; }
    }
        
    public struct RegisterViewModelExistingAction
    {
    	public RegisterViewModelExistingAction(TextEditorViewModel viewModel)
    	{
    		ViewModel = viewModel;
    	}
    	
    	public TextEditorViewModel ViewModel { get; }
    }

    public struct DisposeViewModelAction
    {
    	public DisposeViewModelAction(Key<TextEditorViewModel> viewModelKey)
    	{
    		ViewModelKey = viewModelKey;
    	}
    	
    	public Key<TextEditorViewModel> ViewModelKey { get; }
    }

    public struct SetViewModelWithAction
    {
    	public SetViewModelWithAction(
    		ITextEditorEditContext editContext,
	        Key<TextEditorViewModel> viewModelKey,
	        Func<TextEditorViewModel, TextEditorViewModel> withFunc)
	    {
	    	EditContext = editContext;
	        ViewModelKey = viewModelKey;
	        WithFunc = withFunc;
    	}
    	
        public ITextEditorEditContext EditContext { get; }
        public Key<TextEditorViewModel> ViewModelKey { get; }
        public Func<TextEditorViewModel, TextEditorViewModel> WithFunc { get; }
    }

	public struct SetModelAndViewModelRangeAction
	{
		public SetModelAndViewModelRangeAction(
			ITextEditorEditContext editContext,
			Dictionary<ResourceUri, TextEditorModelModifier?>? modelModifierList,
			Dictionary<Key<TextEditorViewModel>, TextEditorViewModelModifier?>? viewModelModifierList)
		{
			EditContext = editContext;
			ModelModifierList = modelModifierList;
			ViewModelModifierList = viewModelModifierList;
		}
		
        public ITextEditorEditContext EditContext { get; }
        public Dictionary<ResourceUri, TextEditorModelModifier?>? ModelModifierList { get; }
		public Dictionary<Key<TextEditorViewModel>, TextEditorViewModelModifier?>? ViewModelModifierList { get; }
	}
}
