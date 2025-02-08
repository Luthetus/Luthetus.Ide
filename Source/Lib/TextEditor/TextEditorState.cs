using System.Collections.Immutable;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;

namespace Luthetus.TextEditor.RazorLib;

public record TextEditorState
{
	// Move TextEditorState.Main.cs here (2025-02-08)
	private readonly Dictionary<ResourceUri, TextEditorModel> _modelMap = new();
	private readonly Dictionary<Key<TextEditorViewModel>, TextEditorViewModel> _viewModelMap = new();
	
	public (TextEditorModel? TextEditorModel, TextEditorViewModel? TextEditorViewModel) GetModelAndViewModelOrDefault(
		ResourceUri resourceUri, Key<TextEditorViewModel> viewModelKey)
	{
		var inModel = (TextEditorModel?)null;
		var inViewModel = (TextEditorViewModel?)null;
		
		try
		{
			_ = _modelMap.TryGetValue(resourceUri, out inModel);
			_ = _viewModelMap.TryGetValue(viewModelKey, out inViewModel);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return (inModel, inViewModel);
	}
	
	/// <summary>
	/// This overload will lookup the model for the given view model, in the case that one only has access to the viewModelKey.
	/// </summary>
	public (TextEditorModel? Model, TextEditorViewModel? ViewModel) GetModelAndViewModelOrDefault(
		Key<TextEditorViewModel> viewModelKey)
	{
		var inModel = (TextEditorModel?)null;
		var inViewModel = (TextEditorViewModel?)null;
		
		try
		{
			_ = _viewModelMap.TryGetValue(viewModelKey, out inViewModel);
			
			if (inViewModel is not null)
				_ = _modelMap.TryGetValue(inViewModel.ResourceUri, out inModel);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return (inModel, inViewModel);
	}
	
	public TextEditorModel? ModelGetOrDefault(ResourceUri resourceUri)
    {
    	var inModel = (TextEditorModel?)null;
    	
    	try
    	{
    		var exists = _modelMap.TryGetValue(resourceUri, out inModel);
    	}
    	catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return inModel;
    }
    
	/// <summary>
	/// Returns a shallow copy
	/// </summary>
    public Dictionary<ResourceUri, TextEditorModel> ModelGetModels()
    {
    	try
    	{
    		return new Dictionary<ResourceUri, TextEditorModel>(_modelMap);
    	}
    	catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return new();
    }
    
    public int ModelGetModelsCount()
    {
    	try
    	{
    		return _modelMap.Count;
    	}
    	catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return 0;
    }
    
    public ImmutableArray<TextEditorViewModel> ModelGetViewModelsOrEmpty(ResourceUri resourceUri)
    {
    	try
    	{
    		return _viewModelMap.Values
    			.Where(x => x.ResourceUri == resourceUri)
            	.ToImmutableArray();;
    	}
    	catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return ImmutableArray<TextEditorViewModel>.Empty;
    }
    
    public TextEditorViewModel? ViewModelGetOrDefault(Key<TextEditorViewModel> viewModelKey)
    {
    	var inViewModel = (TextEditorViewModel?)null;
    
    	try
    	{
    		var exists = _viewModelMap.TryGetValue(viewModelKey, out inViewModel);
    	}
    	catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
    	return inViewModel;
    }

	/// <summary>
	/// Returns a shallow copy
	/// </summary>
    public Dictionary<Key<TextEditorViewModel>, TextEditorViewModel> ViewModelGetViewModels()
    {
    	try
    	{
    		return new Dictionary<Key<TextEditorViewModel>, TextEditorViewModel>(_viewModelMap);
    	}
    	catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return new();
    }
    
    public int ViewModelGetViewModelsCount()
    {
    	try
    	{
    		return _viewModelMap.Count;
    	}
    	catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return 0;
    }
    
    // Move TextEditorState.Actions.cs here
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
	
	// Move TextEditorState.Reducer.cs here
    public static TextEditorState ReduceRegisterModelAction(
        TextEditorState inState,
        RegisterModelAction registerModelAction)
    {
    	var exists = inState._modelMap.TryGetValue(
    		registerModelAction.Model.ResourceUri, out var inModel);
    	
    	if (exists)
            return inState;
		
		inState._modelMap.Add(
    		registerModelAction.Model.ResourceUri, registerModelAction.Model);
    		
    	return inState with {};
    }

    public static TextEditorState ReduceDisposeModelAction(
        TextEditorState inState,
        DisposeModelAction disposeModelAction)
    {
    	var exists = inState._modelMap.TryGetValue(
    		disposeModelAction.ResourceUri, out var inModel);

        if (!exists)
            return inState;

		inState._modelMap.Remove(disposeModelAction.ResourceUri);
		
		return inState with {};
    }

    public static TextEditorState ReduceSetModelAction(
        TextEditorState inState,
        SetModelAction setModelAction)
    {
    	var exists = inState._modelMap.TryGetValue(
    		setModelAction.ModelModifier.ResourceUri, out var inModel);

        if (!exists)
            return inState;

		inState._modelMap[inModel.ResourceUri] = setModelAction.ModelModifier.ToModel();
    	
    	return inState with {};
    }

    public static TextEditorState ReduceRegisterViewModelAction(
        TextEditorState inState,
        RegisterViewModelAction registerViewModelAction)
    {
    	// The category and ViewModelKey do NOT need to be a compound unique identifier
    	// Only check for the 'ViewModelKey' already existing.
    	//
    	// Category is just a way to filter a list of view models.
    	//
    	// TODO: What is the difference between Category and Group? I'm asking this to myself. If their redundant then get rid of one. Otherwise...
    	//       ...write down the justification for both existing before you forget again.
    	//
    	// I think I made both Category and Group because:
    	// Category describes relationships between view models
    	//
    	// Group is solely meant to provide tab UI.
    	// 	- One can put a view model of any category into a group.
    	// 	- Or one could add dropzone logic that validates the category of a 'being dragged view model'
    	//     	  to ensure it belongs in that group.
    	
        var inViewModel = inState.ViewModelGetOrDefault(registerViewModelAction.ViewModelKey);

        if (inViewModel is not null)
            return inState;

        if (registerViewModelAction.ViewModelKey == Key<TextEditorViewModel>.Empty)
            throw new InvalidOperationException($"Provided {nameof(Key<TextEditorViewModel>)} cannot be {nameof(Key<TextEditorViewModel>)}.{Key<TextEditorViewModel>.Empty}");

        var viewModel = new TextEditorViewModel(
            registerViewModelAction.ViewModelKey,
            registerViewModelAction.ResourceUri,
            registerViewModelAction.TextEditorService,
            registerViewModelAction.Dispatcher,
            registerViewModelAction.DialogService,
            registerViewModelAction.JsRuntime,
            VirtualizationGrid.Empty,
			new TextEditorDimensions(0, 0, 0, 0),
			new ScrollbarDimensions(0, 0, 0, 0, 0),
    		new CharAndLineMeasurements(0, 0),
            false,
            registerViewModelAction.Category);

		inState._viewModelMap.Add(viewModel.ViewModelKey, viewModel);
    	
    	return inState with {};
    }
    
    public static TextEditorState ReduceRegisterViewModelExistingAction(
        TextEditorState inState,
        RegisterViewModelExistingAction registerViewModelExistingAction)
    {
        var inViewModel = inState.ViewModelGetOrDefault(
        	registerViewModelExistingAction.ViewModel.ViewModelKey);

        if (inViewModel is not null)
            return inState;

        if (registerViewModelExistingAction.ViewModel.ViewModelKey == Key<TextEditorViewModel>.Empty)
            throw new InvalidOperationException($"Provided {nameof(Key<TextEditorViewModel>)} cannot be {nameof(Key<TextEditorViewModel>)}.{Key<TextEditorViewModel>.Empty}");

		inState._viewModelMap.Add(
			registerViewModelExistingAction.ViewModel.ViewModelKey,
			registerViewModelExistingAction.ViewModel);
			
    	return inState with {};
    }

    public static TextEditorState ReduceDisposeViewModelAction(
        TextEditorState inState,
        DisposeViewModelAction disposeViewModelAction)
    {
        var inViewModel = inState.ViewModelGetOrDefault(
            disposeViewModelAction.ViewModelKey);

        if (inViewModel is null)
            return inState;
            
		inState._viewModelMap.Remove(inViewModel.ViewModelKey);
		inViewModel.Dispose();
    	return inState with {};
    }

    public static TextEditorState ReduceSetViewModelWithAction(
        TextEditorState inState,
        SetViewModelWithAction setViewModelWithAction)
    {
        var inViewModel = inState.ViewModelGetOrDefault(
            setViewModelWithAction.ViewModelKey);

        if (inViewModel is null)
            return inState;

		var outViewModel = setViewModelWithAction.WithFunc.Invoke(inViewModel);
        inState._viewModelMap[inViewModel.ViewModelKey] = outViewModel;
        return inState with {};
    }

    public static TextEditorState ReduceSetModelAndViewModelRangeAction(
        TextEditorState inState,
        SetModelAndViewModelRangeAction setModelAndViewModelRangeAction)
    {
    	/*
    	Object reference not set to an instance of an object.
			at Luthetus.TextEditor.RazorLib.TextEditors.States.TextEditorState.Reducer.ReduceSetModelAndViewModelRangeAction(TextEditorState inState, SetModelAndViewModelRangeAction setModelAndViewModelRangeAction) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\States\TextEditorState.Reducer.cs:line 174
			at Fluxor.DependencyInjection.Wrappers.ReducerWrapper2.Fluxor.IReducer<TState>.Reduce(TState state, Object action) in C:\Data\Mine\Code\Fluxor\Source\Lib\Fluxor\DependencyInjection\Wrappers\ReducerWrapper.cs:line 11 at Fluxor.Feature1.ReceiveDispatchNotificationFromStore(Object action)
			at Fluxor.Store.DequeueActions() in C:\Data\Mine\Code\Fluxor\Source\Lib\Fluxor\Store.cs:line 290
			at Fluxor.Store.ActionDispatched(Object sender, ActionDispatchedEventArgs e) in C:\Data\Mine\Code\Fluxor\Source\Lib\Fluxor\Store.cs:line 173
			at Fluxor.Dispatcher.DequeueActions() in C:\Data\Mine\Code\Fluxor\Source\Lib\Fluxor\Dispatcher.cs:line 69
			at Fluxor.Dispatcher.Dispatch(Object action) in C:\Data\Mine\Code\Fluxor\Source\Lib\Fluxor\Dispatcher.cs:line 46
			at Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.ScrollbarSection.VERTICAL_HandleOnMouseDownAsync(MouseEventArgs mouseEventArgs) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Displays\Internals\ScrollbarSection.razor.cs:line 194
			at Microsoft.AspNetCore.Components.ComponentBase.CallStateHasChangedOnAsyncCompletion(Task task)
			at Microsoft.AspNetCore.Components.RenderTree.Renderer.GetErrorHandledTask(Task taskToHandle, ComponentState owningComponentState)
			
		If this code throws an exception it can crash the main application if using text editor as a NuGet Package
		so I will put a try-catch here.
		
		I'd as well like to figure out what the issue is but it isn't obvious and this is too risky of a thing to mess up.
		
	    Additional note: this was happening during the solution wide parse.
	    It was just "random" while I was using the IDE during the solution wide parse, sometimes it happened sometimes it didn't.
	    
		-----------------------
		
		I think I get what is happening. For some reason a null is being added to the
		setModelAndViewModelRangeAction.ModelModifierList.
		
		I added 'if (null) continue;' sort of code but I will keep the 'try' 'catch'.
    	*/
    	try
    	{
    		// Models
    		if (setModelAndViewModelRangeAction.ModelModifierList is not null)
    		{
				foreach (var kvpModelModifier in setModelAndViewModelRangeAction.ModelModifierList)
				{
					if (kvpModelModifier.Value is null || !kvpModelModifier.Value.WasModified)
						continue;
					
					// Enumeration was modified shouldn't occur here because only the reducer
					// should be adding or removing, and the reducer is thread safe.
					var exists = inState._modelMap.TryGetValue(
		        		kvpModelModifier.Value.ResourceUri, out var inModel);
		
		            if (!exists)
		                continue;
		                
					inState._modelMap[kvpModelModifier.Value.ResourceUri] = kvpModelModifier.Value.ToModel();
				}
			}
			
			// ViewModels
			if (setModelAndViewModelRangeAction.ViewModelModifierList is not null)
			{
				foreach (var kvpViewModelModifier in setModelAndViewModelRangeAction.ViewModelModifierList)
				{
					if (kvpViewModelModifier.Value is null || !kvpViewModelModifier.Value.WasModified)
						continue;
						
					// Enumeration was modified shouldn't occur here because only the reducer
					// should be adding or removing, and the reducer is thread safe.
					var exists = inState._viewModelMap.TryGetValue(
		        		kvpViewModelModifier.Value.ViewModel.ViewModelKey, out var inViewModel);
		        		
		        	if (!exists)
		                continue;
		
	                inState._viewModelMap[kvpViewModelModifier.Value.ViewModel.ViewModelKey] = kvpViewModelModifier.Value.ViewModel;
				}
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}

        return inState with {};
    }
}
