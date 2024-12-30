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
            
            // This if expression exists below, to check if 'CalculateVirtualizationResult(...)' should be invoked.
            //
            // But, note that these cannot be combined at the bottom, we need to check if an edit
            // reduced the scrollWidth or scrollHeight of the editor's content.
            // 
            // This is done here, so that the 'ScrollWasModified' bool can be set, and downstream if statements will be entered,
            // which go on to scroll the editor.
            if (viewModelModifier.ShouldReloadVirtualizationResult)
			{
				ValidateMaximumScrollLeftAndScrollTop(editContext, viewModelModifier, textEditorDimensionsChanged: false);
			}

            if (viewModelModifier.ScrollWasModified)
            {
            	// TODO: If the API is self validating then this explicit final validation wouldn't be needed?
            	/*
            	var validateScrollbarDimensions = viewModelModifier.ViewModel.ScrollbarDimensions;
            
            	validateScrollbarDimensions = viewModelModifier.ViewModel.ScrollbarDimensions.WithSetScrollLeft(
        			(int)validateScrollbarDimensions.ScrollLeft,
        			viewModelModifier.ViewModel.TextEditorDimensions);
            			
            	validateScrollbarDimensions = validateScrollbarDimensions.WithSetScrollTop(
        			(int)validateScrollbarDimensions.ScrollTop,
        			viewModelModifier.ViewModel.TextEditorDimensions);
        			
            	viewModelModifier.ViewModel = viewModelModifier.ViewModel with
				{
					ScrollbarDimensions = validateScrollbarDimensions
				};
				*/
            
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
            		var firstEntryTop = firstEntry.LineIndex * viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight;
            		
            		if (viewModelModifier.ViewModel.ScrollbarDimensions.ScrollTop < firstEntryTop)
            		{
            			viewModelModifier.ShouldReloadVirtualizationResult = true;
            		}
            		else
            		{
            			var bigTop = viewModelModifier.ViewModel.ScrollbarDimensions.ScrollTop +
            				viewModelModifier.ViewModel.TextEditorDimensions.Height;
            				
            			var imaginaryLastEntry = viewModelModifier.ViewModel.VirtualizationResult.EntryList.Last();
            			var imaginaryLastEntryTop = (imaginaryLastEntry.LineIndex + 1) * viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight;
            				
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
	
	/// <summary>
	/// The argument 'bool textEditorDimensionsChanged' was added on (2024-09-20).
	/// 
	/// The issue is that this method was originally written for fixing the scrollLeft or scrollTop
	/// when the scrollWidth or scrollHeight changed to a smaller value.
	///
	/// The if statements therefore check that the originalScrollWidth was higher,
	/// because some invokers of this method won't need to take time doing this calculation.
	///
	/// For example, a pure insertion of text won't need to run this method. So the if statements
	/// allow for a quick exit.
	///
	/// But, it was discovered that this same logic is needed when the text editor width changes.
	///
	/// The text editor width changing results in a very specific event firing. So if we could
	/// just have a bool here to say, "I'm that specific event" then we can know that the width changed.
	/// 
	/// Because we cannot track the before and after of the width from this method since it already was changed.
	/// We need the specific event to instead tell us that it had changed.
	/// 
	/// So, the bool is a bit hacky.
	/// </summary>
	public void ValidateMaximumScrollLeftAndScrollTop(
		ITextEditorEditContext editContext,
		TextEditorViewModelModifier viewModelModifier,
		bool textEditorDimensionsChanged)
	{	
		var modelModifier = editContext.GetModelModifier(viewModelModifier.ViewModel.ResourceUri);
    	
    	if (modelModifier is null)
    		return;
		
		var originalScrollWidth = viewModelModifier.ViewModel.ScrollbarDimensions.ScrollWidth;
		var originalScrollHeight = viewModelModifier.ViewModel.ScrollbarDimensions.ScrollHeight;
	
		var totalWidth = (int)Math.Ceiling(modelModifier.MostCharactersOnASingleLineTuple.lineLength *
			viewModelModifier.ViewModel.CharAndLineMeasurements.CharacterWidth);

		// Account for any tab characters on the 'MostCharactersOnASingleLineTuple'
		//
		// TODO: This code is not fully correct...
		//       ...if the longest line is 50 non-tab characters,
		//       and the second longest line is 49 tab characters,
		//       this code will erroneously take the '50' non-tab characters
		//       to be the longest line.
		{
			var lineIndex = modelModifier.MostCharactersOnASingleLineTuple.lineIndex;
			var longestLineInformation = modelModifier.GetLineInformation(lineIndex);

			var tabCountOnLongestLine = modelModifier.GetTabCountOnSameLineBeforeCursor(
				longestLineInformation.Index,
				longestLineInformation.LastValidColumnIndex);

			// 1 of the character width is already accounted for
			var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

			totalWidth += (int)Math.Ceiling(extraWidthPerTabKey *
				tabCountOnLongestLine *
				viewModelModifier.ViewModel.CharAndLineMeasurements.CharacterWidth);
		}

		var totalHeight = (int)Math.Ceiling(modelModifier.LineEndList.Count *
			viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight);

		// Add vertical margin so the user can scroll beyond the final line of content
		int marginScrollHeight;
		{
			var percentOfMarginScrollHeightByPageUnit = 0.4;

			marginScrollHeight = (int)Math.Ceiling(viewModelModifier.ViewModel.TextEditorDimensions.Height * percentOfMarginScrollHeightByPageUnit);
			totalHeight += marginScrollHeight;
		}

		viewModelModifier.ViewModel = viewModelModifier.ViewModel with
		{
			ScrollbarDimensions = viewModelModifier.ViewModel.ScrollbarDimensions with
			{
				ScrollWidth = totalWidth,
				ScrollHeight = totalHeight,
				MarginScrollHeight = marginScrollHeight
			},
		};
		
		var validateScrollbarDimensions = viewModelModifier.ViewModel.ScrollbarDimensions;
		
		if (originalScrollWidth > viewModelModifier.ViewModel.ScrollbarDimensions.ScrollWidth ||
			textEditorDimensionsChanged)
		{
			validateScrollbarDimensions = viewModelModifier.ViewModel.ScrollbarDimensions.WithSetScrollLeft(
				(int)validateScrollbarDimensions.ScrollLeft,
				viewModelModifier.ViewModel.TextEditorDimensions);
		}
		
		if (originalScrollHeight > viewModelModifier.ViewModel.ScrollbarDimensions.ScrollHeight ||
			textEditorDimensionsChanged)
		{
			validateScrollbarDimensions = validateScrollbarDimensions.WithSetScrollTop(
				(int)validateScrollbarDimensions.ScrollTop,
				viewModelModifier.ViewModel.TextEditorDimensions);
			
			// The scrollLeft currently does not have any margin. Therefore subtracting the margin isn't needed.
			//
			// For scrollTop however, if one does not subtract the MarginScrollHeight in the case of
			// 'textEditorDimensionsChanged'
			//
			// Then a "void" will render at the top portion of the text editor, seemingly the size
			// of the MarginScrollHeight.
			if (textEditorDimensionsChanged &&
				viewModelModifier.ViewModel.ScrollbarDimensions.ScrollTop != validateScrollbarDimensions.ScrollTop)
			{
				validateScrollbarDimensions = validateScrollbarDimensions.WithSetScrollTop(
					(int)validateScrollbarDimensions.ScrollTop - (int)validateScrollbarDimensions.MarginScrollHeight,
					viewModelModifier.ViewModel.TextEditorDimensions);
			}
		}
		
		var changeOccurred =
			viewModelModifier.ViewModel.ScrollbarDimensions.ScrollLeft != validateScrollbarDimensions.ScrollLeft ||
			viewModelModifier.ViewModel.ScrollbarDimensions.ScrollTop != validateScrollbarDimensions.ScrollTop;
		
		if (changeOccurred)
		{
			viewModelModifier.ScrollWasModified = true;
			
			viewModelModifier.ViewModel = viewModelModifier.ViewModel with
			{
				ScrollbarDimensions = validateScrollbarDimensions
			};
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
			var resourceUri = new ResourceUri(absolutePath);
			var actualViewModelKey = await CommonLogic_OpenInEditorAsync(
				resourceUri,
				shouldSetFocusToEditor,
				category,
				preferredViewModelKey);
				
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
				
				viewModelModifier.ViewModel.UnsafeState.ShouldRevealCursor = true;
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
	
	public async Task OpenInEditorAsync(
		string absolutePath,
		bool shouldSetFocusToEditor,
		int? lineIndex,
		int? columnIndex,
		Category category,
		Key<TextEditorViewModel> preferredViewModelKey)
	{
		try
		{
			// Standardize Resource Uri
			if (TextEditorConfig.AbsolutePathStandardizeFunc is null)
				return;
				
			var standardizedFilePathString = await TextEditorConfig.AbsolutePathStandardizeFunc
				.Invoke(absolutePath, _serviceProvider)
				.ConfigureAwait(false);
				
			var resourceUri = new ResourceUri(standardizedFilePathString);

			var actualViewModelKey = await CommonLogic_OpenInEditorAsync(
				resourceUri,
				shouldSetFocusToEditor,
				category,
				preferredViewModelKey);
				
			// Move cursor
			if (lineIndex is null && columnIndex is null)
				return; // Leave the cursor unchanged if the argument is null
			PostUnique(nameof(OpenInEditorAsync), editContext =>
			{
				var modelModifier = editContext.GetModelModifier(resourceUri);
				var viewModelModifier = editContext.GetViewModelModifier(actualViewModelKey);
				var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
				var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
		
				if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
					return Task.CompletedTask;
			
				if (lineIndex is not null)
					primaryCursorModifier.LineIndex = lineIndex.Value;
				if (columnIndex is not null)
					primaryCursorModifier.ColumnIndex = columnIndex.Value;
				
				if (primaryCursorModifier.LineIndex > modelModifier.LineCount - 1)
					primaryCursorModifier.LineIndex = modelModifier.LineCount - 1;
				
				var lineInformation = modelModifier.GetLineInformation(primaryCursorModifier.LineIndex);
				
				if (primaryCursorModifier.ColumnIndex > lineInformation.LastValidColumnIndex)
					primaryCursorModifier.SetColumnIndexAndPreferred(lineInformation.LastValidColumnIndex);
					
				viewModelModifier.ViewModel.UnsafeState.ShouldRevealCursor = true;
				
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
	
	/// <summary>
	/// Returns Key<TextEditorViewModel>.Empty if it failed to open in editor.
	/// Returns the ViewModel's key (non Key<TextEditorViewModel>.Empty value) if it successfully opened in editor.
	/// </summary>
	private async Task<Key<TextEditorViewModel>> CommonLogic_OpenInEditorAsync(
		ResourceUri resourceUri,
		bool shouldSetFocusToEditor,
		Category category,
		Key<TextEditorViewModel> preferredViewModelKey)
	{
		// RegisterModelFunc
		if (TextEditorConfig.RegisterModelFunc is null)
			return Key<TextEditorViewModel>.Empty;
		await TextEditorConfig.RegisterModelFunc.Invoke(new RegisterModelArgs(resourceUri, _serviceProvider)).ConfigureAwait(false);
	
		// TryRegisterViewModelFunc
		if (TextEditorConfig.TryRegisterViewModelFunc is null)
			return Key<TextEditorViewModel>.Empty;
		var actualViewModelKey = await TextEditorConfig.TryRegisterViewModelFunc.Invoke(new TryRegisterViewModelArgs(
			preferredViewModelKey, resourceUri, category, shouldSetFocusToEditor, _serviceProvider)).ConfigureAwait(false);
	
		// TryShowViewModelFunc
		if (actualViewModelKey == Key<TextEditorViewModel>.Empty || TextEditorConfig.TryShowViewModelFunc is null)
			return Key<TextEditorViewModel>.Empty;
		await TextEditorConfig.TryShowViewModelFunc.Invoke(new TryShowViewModelArgs(
			actualViewModelKey, Key<TextEditorGroup>.Empty, shouldSetFocusToEditor, _serviceProvider)).ConfigureAwait(false);
		
		return actualViewModelKey;
	}
}
