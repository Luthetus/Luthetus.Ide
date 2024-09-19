using System.Collections.Immutable;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Dimensions.States;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.Edits.States;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;

namespace Luthetus.TextEditor.RazorLib;

public partial class TextEditorService : ITextEditorService
{
    /// <summary>
    /// See explanation of this field at: <see cref="TextEditorAuthenticatedAction"/>
    /// </summary>
    public static readonly Key<TextEditorAuthenticatedAction> AuthenticatedActionKey = new(Guid.Parse("13831968-9b10-46d1-8d47-842b78238d6a"));

    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDispatcher _dispatcher;
    private readonly IDialogService _dialogService;
    private readonly ITextEditorRegistryWrap _textEditorRegistryWrap;
    private readonly IStorageService _storageService;
    // TODO: Perhaps do not reference IJSRuntime but instead wrap it in a 'IUiProvider' or something like that. The 'IUiProvider' would then expose methods that allow the TextEditorViewModel to adjust the scrollbars. 
    private readonly IJSRuntime _jsRuntime;
    private readonly CommonBackgroundTaskApi _commonBackgroundTaskApi;
    private readonly IServiceProvider _serviceProvider;

    public TextEditorService(
        IState<TextEditorState> textEditorStateWrap,
        IState<TextEditorGroupState> groupStateWrap,
        IState<TextEditorDiffState> diffStateWrap,
        IState<ThemeState> themeStateWrap,
        IState<TextEditorOptionsState> optionsStateWrap,
        IState<TextEditorFindAllState> findAllStateWrap,
        IBackgroundTaskService backgroundTaskService,
        LuthetusTextEditorConfig textEditorConfig,
        ITextEditorRegistryWrap textEditorRegistryWrap,
        IStorageService storageService,
        IJSRuntime jsRuntime,
        CommonBackgroundTaskApi commonBackgroundTaskApi,
        IDispatcher dispatcher,
        IDialogService dialogService,
		IAutocompleteIndexer autocompleteIndexer,
		IAutocompleteService autocompleteService,
		IState<AppDimensionState> appDimensionStateWrap,
		IServiceProvider serviceProvider)
    {
        TextEditorStateWrap = textEditorStateWrap;
        GroupStateWrap = groupStateWrap;
        DiffStateWrap = diffStateWrap;
        ThemeStateWrap = themeStateWrap;
        OptionsStateWrap = optionsStateWrap;
        FindAllStateWrap = findAllStateWrap;
		AppDimensionStateWrap = appDimensionStateWrap;
		_serviceProvider = serviceProvider;

        _backgroundTaskService = backgroundTaskService;
        TextEditorConfig = textEditorConfig;
        _textEditorRegistryWrap = textEditorRegistryWrap;
        _storageService = storageService;
        _jsRuntime = jsRuntime;
		JsRuntimeTextEditorApi = _jsRuntime.GetLuthetusTextEditorApi();
		JsRuntimeCommonApi = _jsRuntime.GetLuthetusCommonApi();
        _commonBackgroundTaskApi = commonBackgroundTaskApi;
        _dispatcher = dispatcher;
        _dialogService = dialogService;

		AutocompleteIndexer = autocompleteIndexer;
		AutocompleteService = autocompleteService;

        ModelApi = new TextEditorModelApi(this, _textEditorRegistryWrap.DecorationMapperRegistry, _textEditorRegistryWrap.CompilerServiceRegistry, _backgroundTaskService, _dispatcher);
        ViewModelApi = new TextEditorViewModelApi(this, _backgroundTaskService, TextEditorStateWrap, _jsRuntime, _dispatcher, _dialogService);
        GroupApi = new TextEditorGroupApi(this, _dispatcher, _dialogService, _jsRuntime);
        DiffApi = new TextEditorDiffApi(this, _dispatcher);
        OptionsApi = new TextEditorOptionsApi(this, TextEditorConfig, _storageService, _commonBackgroundTaskApi, _dispatcher);
    }

    public IState<TextEditorState> TextEditorStateWrap { get; }
    public IState<TextEditorGroupState> GroupStateWrap { get; }
    public IState<TextEditorDiffState> DiffStateWrap { get; }
    public IState<ThemeState> ThemeStateWrap { get; }
    public IState<TextEditorOptionsState> OptionsStateWrap { get; }
    public IState<TextEditorFindAllState> FindAllStateWrap { get; }
	public IState<AppDimensionState> AppDimensionStateWrap { get; }

	public LuthetusTextEditorJavaScriptInteropApi JsRuntimeTextEditorApi { get; }
	public LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi { get; }
	public IAutocompleteIndexer AutocompleteIndexer { get; }
	public IAutocompleteService AutocompleteService { get; }
	public LuthetusTextEditorConfig TextEditorConfig { get; }

#if DEBUG
    public string StorageKey => "luth_te_text-editor-options-debug";
#else
    public string StorageKey => "luth_te_text-editor-options";
#endif

    public string ThemeCssClassString => ThemeStateWrap.Value.ThemeList.FirstOrDefault(
        x => x.Key == OptionsStateWrap.Value.Options.CommonOptions.ThemeKey)
        ?.CssClassString
            ?? ThemeFacts.VisualStudioDarkThemeClone.CssClassString;

    public ITextEditorModelApi ModelApi { get; }
    public ITextEditorViewModelApi ViewModelApi { get; }
    public ITextEditorGroupApi GroupApi { get; }
    public ITextEditorDiffApi DiffApi { get; }
    public ITextEditorOptionsApi OptionsApi { get; }

    public void PostUnique(
        string name,
        Func<ITextEditorEditContext, Task> textEditorFunc)
    {
        Post(new UniqueTextEditorWork(
            name,
            textEditorFunc));
    }

    public void PostRedundant(
        string name,
		ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        Func<ITextEditorEditContext, Task> textEditorFunc)
    {
        Post(new RedundantTextEditorWork(
            name,
			resourceUri,
            viewModelKey,
            textEditorFunc));
    }

    public void Post(ITextEditorWork work)
    {
        work.EditContext = new TextEditorEditContext(
            this,
            AuthenticatedActionKey);

        _backgroundTaskService.Enqueue(work);
    }
    
    public Task PostAsync(ITextEditorWork work)
    {
    	work.EditContext = new TextEditorEditContext(
            this,
            AuthenticatedActionKey);

        return _backgroundTaskService.EnqueueAsync(work);
    }

	public async Task FinalizePost(ITextEditorEditContext editContext)
	{
		var modelModifierNeedRenderList = new List<TextEditorModelModifier>();
		var viewModelModifierNeedRenderList = new List<TextEditorViewModelModifier>();

        foreach (var modelModifier in editContext.ModelCache.Values)
        {
            if (modelModifier is null || !modelModifier.WasModified)
                continue;

			modelModifierNeedRenderList.Add(modelModifier);

            var viewModelBag = editContext.TextEditorService.ModelApi.GetViewModelsOrEmpty(modelModifier.ResourceUri);

            foreach (var viewModel in viewModelBag)
            {
                // Invoking 'GetViewModelModifier' marks the view model to be updated.
                var viewModelModifier = editContext.GetViewModelModifier(viewModel.ViewModelKey);

				if (!viewModelModifier.ShouldReloadVirtualizationResult)
					viewModelModifier.ShouldReloadVirtualizationResult = modelModifier.ShouldReloadVirtualizationResult;
            }

            if (modelModifier.WasDirty != modelModifier.IsDirty)
            {
                if (modelModifier.IsDirty)
                    _dispatcher.Dispatch(new DirtyResourceUriState.AddDirtyResourceUriAction(modelModifier.ResourceUri));
                else
                    _dispatcher.Dispatch(new DirtyResourceUriState.RemoveDirtyResourceUriAction(modelModifier.ResourceUri));
            }
        }

        foreach (var viewModelModifier in editContext.ViewModelCache.Values)
        {
            if (viewModelModifier is null || !viewModelModifier.WasModified)
                return;

			viewModelModifierNeedRenderList.Add(viewModelModifier);

            var successCursorModifierBag = editContext.CursorModifierBagCache.TryGetValue(
                viewModelModifier.ViewModel.ViewModelKey,
                out var cursorModifierBag);

            if (successCursorModifierBag && cursorModifierBag is not null)
            {
                viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                {
                    CursorList = cursorModifierBag.List
                        .Select(x => x.ToCursor())
                        .ToImmutableArray()
                };
            }
            
            if (viewModelModifier.ViewModel.UnsafeState.ShouldRevealCursor)
            {
            	var modelModifier = editContext.GetModelModifier(viewModelModifier.ViewModel.ResourceUri);
            	cursorModifierBag ??= editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
            	var cursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
            	
            	if (modelModifier is not null)
            	{
            		ViewModelApi.RevealCursor(
	            		editContext,
				        modelModifier,
				        viewModelModifier,
				        cursorModifierBag,
				        cursorModifier);
            	}
            }

            if (viewModelModifier.ScrollWasModified)
            {
            	// The 'SetScroll...' methods validate the resulting value,
            	// so setting each dimension to itself will trigger the validation.
            	var validateScrollbarDimensions = viewModelModifier.ViewModel.ScrollbarDimensions
            		.SetScrollLeft(
            			(int)viewModelModifier.ViewModel.ScrollbarDimensions.ScrollLeft,
            			viewModelModifier.ViewModel.TextEditorDimensions)
            		.SetScrollTop(
            			(int)viewModelModifier.ViewModel.ScrollbarDimensions.ScrollTop,
            			viewModelModifier.ViewModel.TextEditorDimensions);
            
            	viewModelModifier.ViewModel = viewModelModifier.ViewModel with
				{
					ScrollbarDimensions = validateScrollbarDimensions
				};
            
                await JsRuntimeTextEditorApi
		            .SetScrollPosition(
		                viewModelModifier.ViewModel.BodyElementId,
		                viewModelModifier.ViewModel.GutterElementId,
		                viewModelModifier.ViewModel.ScrollbarDimensions.ScrollLeft,
		                viewModelModifier.ViewModel.ScrollbarDimensions.ScrollTop)
		            .ConfigureAwait(false);
            }
            
            if (!viewModelModifier.ShouldReloadVirtualizationResult &&
            	viewModelModifier.ScrollWasModified)
            {
            	// If not already going to reload virtualization result,
            	// then check if the virtualization needs to be refreshed due to a
            	// change in scroll position.
            	//
            	// This code only needs to run if the scrollbar was modified.
            	
            	if (viewModelModifier.ViewModel.VirtualizationResult.EntryList.Length > 0)
            	{
            		var firstEntry = viewModelModifier.ViewModel.VirtualizationResult.EntryList.First();
            		var firstEntryTop = firstEntry.Index * viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight;

            		
            		if (viewModelModifier.ViewModel.ScrollbarDimensions.ScrollTop < firstEntryTop)
            		{
            			viewModelModifier.ShouldReloadVirtualizationResult = true;
            		}
            		else
            		{
            			var bigTop = viewModelModifier.ViewModel.ScrollbarDimensions.ScrollTop +
            				viewModelModifier.ViewModel.TextEditorDimensions.Height;
            				
            			var imaginaryLastEntry = viewModelModifier.ViewModel.VirtualizationResult.EntryList.Last();
            			var imaginaryLastEntryTop = (imaginaryLastEntry.Index + 1) * viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight;
            				
            			if (bigTop > imaginaryLastEntryTop)
            				viewModelModifier.ShouldReloadVirtualizationResult = true;
            		}
            	}
            	
            	// A check for horizontal virtualization still needs to be done.
            	//
            	// If we didn't already determine the necessity of calculating the virtualization
            	// result when checking the vertical virtualization, then we check horizontal.
            	if (!viewModelModifier.ShouldReloadVirtualizationResult)
            	{
            		// low end plus width of it
            	
            		var leftBoundary = viewModelModifier.ViewModel.VirtualizationResult.LeftVirtualizationBoundary;
            		var scrollLeft = viewModelModifier.ViewModel.ScrollbarDimensions.ScrollLeft;
            		
            		if (scrollLeft < (leftBoundary.LeftInPixels + leftBoundary.WidthInPixels))
            		{
            			viewModelModifier.ShouldReloadVirtualizationResult = true;
            		}
            		else
            		{
            			var rightBoundary = viewModelModifier.ViewModel.VirtualizationResult.RightVirtualizationBoundary;
						var bigLeft = scrollLeft + viewModelModifier.ViewModel.TextEditorDimensions.Width;
            			
            			if (bigLeft > rightBoundary.LeftInPixels)
            			{
            				viewModelModifier.ShouldReloadVirtualizationResult = true;
            			}
            		}
            	}
            }

			if (viewModelModifier.ShouldReloadVirtualizationResult)
			{
				// TODO: This 'CalculateVirtualizationResultFactory' invocation is horrible for performance.
	            editContext.TextEditorService.ViewModelApi.CalculateVirtualizationResult(
	            	editContext,
	            	editContext.GetModelModifier(viewModelModifier.ViewModel.ResourceUri),
			        viewModelModifier,
			        CancellationToken.None);
			}
            
			_dispatcher.Dispatch(new TextEditorState.SetModelAndViewModelRangeAction(
                editContext.AuthenticatedActionKey,
                editContext,
                modelModifierNeedRenderList,
				viewModelModifierNeedRenderList));
        }
	}
	
	public async Task OpenInEditorAsync(
		string absolutePath,
		bool shouldSetFocusToEditor,
		int? cursorPositionIndex,
		Category category,
		Key<TextEditorViewModel> preferredViewModelKey)
	{
		try
		{
			// RegisterModelFunc
			if (TextEditorConfig.RegisterModelFunc is null)
				return;
			var resourceUri = new ResourceUri(absolutePath);
			await TextEditorConfig.RegisterModelFunc.Invoke(new RegisterModelArgs(resourceUri, _serviceProvider)).ConfigureAwait(false);
		
			// TryRegisterViewModelFunc
			if (TextEditorConfig.TryRegisterViewModelFunc is null)
				return;
			var actualViewModelKey = await TextEditorConfig.TryRegisterViewModelFunc.Invoke(new TryRegisterViewModelArgs(
				preferredViewModelKey, resourceUri, category, shouldSetFocusToEditor, _serviceProvider)).ConfigureAwait(false);
		
			// TryShowViewModelFunc
			if (actualViewModelKey == Key<TextEditorViewModel>.Empty || TextEditorConfig.TryShowViewModelFunc is null)
				return;
			await TextEditorConfig.TryShowViewModelFunc.Invoke(new TryShowViewModelArgs(
				actualViewModelKey, Key<TextEditorGroup>.Empty, shouldSetFocusToEditor, _serviceProvider)).ConfigureAwait(false);
				
			// Move cursor
			if (cursorPositionIndex is null)
				return; // Leave the cursor unchanged if the argument is null
			PostUnique(nameof(OpenInEditorAsync), editContext =>
			{
				var modelModifier = editContext.GetModelModifier(resourceUri);
				var viewModelModifier = editContext.GetViewModelModifier(actualViewModelKey);
				var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
				var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
		
				if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
					return Task.CompletedTask;
			
				var lineAndColumnIndices = modelModifier.GetLineAndColumnIndicesFromPositionIndex(cursorPositionIndex.Value);
					
				primaryCursorModifier.LineIndex = lineAndColumnIndices.lineIndex;
				primaryCursorModifier.ColumnIndex = lineAndColumnIndices.columnIndex;
				return Task.CompletedTask;
			});
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			// One would never want a failed attempt at opening a text file to cause a fatal exception.
			// TODO: Perhaps add a notification? Perhaps 'throw' then add handling in the callers? But again, this should never cause a fatal exception.
		}
	}
}
