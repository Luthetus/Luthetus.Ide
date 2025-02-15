using System.Collections.Immutable;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;

namespace Luthetus.TextEditor.RazorLib;

public partial class TextEditorService : ITextEditorService
{
    private readonly object _stateModificationLock = new();

    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IPanelService _panelService;
    private readonly IDialogService _dialogService;
    private readonly IDirtyResourceUriService _dirtyResourceUriService;
    private readonly ITextEditorRegistryWrap _textEditorRegistryWrap;
    private readonly IStorageService _storageService;
    // TODO: Perhaps do not reference IJSRuntime but instead wrap it in a 'IUiProvider' or something like that. The 'IUiProvider' would then expose methods that allow the TextEditorViewModel to adjust the scrollbars. 
    private readonly IJSRuntime _jsRuntime;
    private readonly CommonBackgroundTaskApi _commonBackgroundTaskApi;
    private readonly IServiceProvider _serviceProvider;

    public TextEditorService(
        IFindAllService findAllService,
        IDirtyResourceUriService dirtyResourceUriService,
        IThemeService themeService,
        IBackgroundTaskService backgroundTaskService,
        LuthetusTextEditorConfig textEditorConfig,
        ITextEditorRegistryWrap textEditorRegistryWrap,
        IStorageService storageService,
        IJSRuntime jsRuntime,
        CommonBackgroundTaskApi commonBackgroundTaskApi,
        IPanelService panelService,
        IDialogService dialogService,
        IContextService contextService,
		IAutocompleteIndexer autocompleteIndexer,
		IAutocompleteService autocompleteService,
		IAppDimensionService appDimensionService,
		IServiceProvider serviceProvider)
    {
    	TextEditorWorker = new(this);
    
		AppDimensionService = appDimensionService;
		_serviceProvider = serviceProvider;

        FindAllService = findAllService;
        _dirtyResourceUriService = dirtyResourceUriService;
        ThemeService = themeService;
        _backgroundTaskService = backgroundTaskService;
        TextEditorConfig = textEditorConfig;
        _textEditorRegistryWrap = textEditorRegistryWrap;
        _storageService = storageService;
        _jsRuntime = jsRuntime;
		JsRuntimeTextEditorApi = _jsRuntime.GetLuthetusTextEditorApi();
		JsRuntimeCommonApi = _jsRuntime.GetLuthetusCommonApi();
        _commonBackgroundTaskApi = commonBackgroundTaskApi;
        _dialogService = dialogService;

		AutocompleteIndexer = autocompleteIndexer;
		AutocompleteService = autocompleteService;

        ModelApi = new TextEditorModelApi(this, _textEditorRegistryWrap, _backgroundTaskService);
        ViewModelApi = new TextEditorViewModelApi(this, _backgroundTaskService, _jsRuntime, _dialogService);
        GroupApi = new TextEditorGroupApi(this, _panelService, _dialogService, _jsRuntime);
        DiffApi = new TextEditorDiffApi(this);
        OptionsApi = new TextEditorOptionsApi(this, TextEditorConfig, _storageService, _dialogService, contextService, _commonBackgroundTaskApi);
        
        TextEditorState = new();
    }

    public IThemeService ThemeService { get; }
    public IAppDimensionService AppDimensionService { get; }
    public IFindAllService FindAllService { get; }

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

    public string ThemeCssClassString => ThemeService.GetThemeState().ThemeList.FirstOrDefault(
        x => x.Key == OptionsApi.GetTextEditorOptionsState().Options.CommonOptions.ThemeKey)
        ?.CssClassString
            ?? ThemeFacts.VisualStudioDarkThemeClone.CssClassString;

    public ITextEditorModelApi ModelApi { get; }
    public ITextEditorViewModelApi ViewModelApi { get; }
    public ITextEditorGroupApi GroupApi { get; }
    public ITextEditorDiffApi DiffApi { get; }
    public ITextEditorOptionsApi OptionsApi { get; }
    
    public TextEditorState TextEditorState { get; }
    
    public TextEditorWorker TextEditorWorker { get; }
    
    public IBackgroundTaskService BackgroundTaskService => _backgroundTaskService;
    
    public event Action? TextEditorStateChanged;

	public async ValueTask FinalizePost(ITextEditorEditContext editContext)
	{
		if (editContext.ModelCache is not null)
		{
	        foreach (var modelModifier in editContext.ModelCache.Values)
	        {
	            if (modelModifier is null || !modelModifier.WasModified)
	                continue;
	
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
	                    _dirtyResourceUriService.AddDirtyResourceUri(modelModifier.ResourceUri);
	                else
	                    _dirtyResourceUriService.RemoveDirtyResourceUri(modelModifier.ResourceUri);
	            }
	        }
		}
		
		if (editContext.ViewModelCache is not null)
		{
	        foreach (var viewModelModifier in editContext.ViewModelCache.Values)
	        {
	            if (viewModelModifier is null || !viewModelModifier.WasModified)
	                return;
	
				bool successCursorModifierBag;
				CursorModifierBagTextEditor cursorModifierBag;
				
				if (editContext.CursorModifierBagCache is null)
				{
					successCursorModifierBag = false;
					cursorModifierBag = default;
				}
				else
				{
		            successCursorModifierBag = editContext.CursorModifierBagCache.TryGetValue(
		                viewModelModifier.ViewModel.ViewModelKey,
		                out cursorModifierBag);
		        }
	
	            if (successCursorModifierBag && cursorModifierBag.ConstructorWasInvoked)
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
	            	
	            	if (!cursorModifierBag.ConstructorWasInvoked)
	            		cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
	            	
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
	        }
	    }
	    
	    SetModelAndViewModelRange(
	        editContext,
	        editContext.ModelCache,
			editContext.ViewModelCache);
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
			TextEditorWorker.PostUnique(nameof(OpenInEditorAsync), editContext =>
			{
				var modelModifier = editContext.GetModelModifier(resourceUri);
				var viewModelModifier = editContext.GetViewModelModifier(actualViewModelKey);
				var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
				var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
		
				if (modelModifier is null || viewModelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
					return ValueTask.CompletedTask;
			
				var lineAndColumnIndices = modelModifier.GetLineAndColumnIndicesFromPositionIndex(cursorPositionIndex.Value);
					
				primaryCursorModifier.LineIndex = lineAndColumnIndices.lineIndex;
				primaryCursorModifier.ColumnIndex = lineAndColumnIndices.columnIndex;
				
				viewModelModifier.ViewModel.UnsafeState.ShouldRevealCursor = true;
				return ValueTask.CompletedTask;
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
			TextEditorWorker.PostUnique(nameof(OpenInEditorAsync), editContext =>
			{
				var modelModifier = editContext.GetModelModifier(resourceUri);
				var viewModelModifier = editContext.GetViewModelModifier(actualViewModelKey);
				var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
				var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
		
				if (modelModifier is null || viewModelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
					return ValueTask.CompletedTask;
			
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
				
				return ValueTask.CompletedTask;
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
	
	// Move TextEditorState.Reducer.cs here
	public void RegisterModel(TextEditorModel model)
	{
		lock (_stateModificationLock)
		{
			var inState = TextEditorState;
	
			var exists = inState._modelMap.TryGetValue(
				model.ResourceUri, out var inModel);
		
			if (exists)
                goto finalize;
		
			inState._modelMap.Add(model.ResourceUri, model);
		
			goto finalize;
        }

		finalize:
        TextEditorStateChanged?.Invoke();
    }

	public void DisposeModel(ResourceUri resourceUri)
	{
		lock (_stateModificationLock)
		{
			var inState = TextEditorState;

			var exists = inState._modelMap.TryGetValue(
				resourceUri, out var inModel);

			if (!exists)
                goto finalize;

			inState._modelMap.Remove(resourceUri);

            goto finalize;
        }

        finalize:
        TextEditorStateChanged?.Invoke();
    }
	
	public void SetModel(
	    ITextEditorEditContext editContext,
	    TextEditorModelModifier modelModifier)
	{
		lock (_stateModificationLock)
		{
			var inState = TextEditorState;

			var exists = inState._modelMap.TryGetValue(
				modelModifier.ResourceUri, out var inModel);

			if (!exists)
                goto finalize;

			inState._modelMap[inModel.ResourceUri] = modelModifier.ToModel();

            goto finalize;
        }

        finalize:
        TextEditorStateChanged?.Invoke();
    }
	
	public void RegisterViewModel(
	    Key<TextEditorViewModel> viewModelKey,
	    ResourceUri resourceUri,
	    Category category,
	    ITextEditorService textEditorService,
	    IDialogService dialogService,
	    IJSRuntime jsRuntime)
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

		lock (_stateModificationLock)
		{
			var inState = TextEditorState;

			var inViewModel = inState.ViewModelGetOrDefault(viewModelKey);

			if (inViewModel is not null)
                goto finalize;

			if (viewModelKey == Key<TextEditorViewModel>.Empty)
				throw new InvalidOperationException($"Provided {nameof(Key<TextEditorViewModel>)} cannot be {nameof(Key<TextEditorViewModel>)}.{Key<TextEditorViewModel>.Empty}");

			var viewModel = new TextEditorViewModel(
				viewModelKey,
				resourceUri,
				textEditorService,
				_panelService,
				dialogService,
				jsRuntime,
				VirtualizationGrid.Empty,
				new TextEditorDimensions(0, 0, 0, 0),
				new ScrollbarDimensions(0, 0, 0, 0, 0),
				new CharAndLineMeasurements(0, 0),
				false,
				category);

			inState._viewModelMap.Add(viewModel.ViewModelKey, viewModel);

            goto finalize;
        }

        finalize:
        TextEditorStateChanged?.Invoke();
    }
	
	public void RegisterViewModelExisting(
	    TextEditorViewModel viewModel)
	{
		lock (_stateModificationLock)
		{
			var inState = TextEditorState;

			var inViewModel = inState.ViewModelGetOrDefault(viewModel.ViewModelKey);

			if (inViewModel is not null)
                goto finalize;

			if (viewModel.ViewModelKey == Key<TextEditorViewModel>.Empty)
				throw new InvalidOperationException($"Provided {nameof(Key<TextEditorViewModel>)} cannot be {nameof(Key<TextEditorViewModel>)}.{Key<TextEditorViewModel>.Empty}");

			inState._viewModelMap.Add(viewModel.ViewModelKey, viewModel);

            goto finalize;
        }

        finalize:
        TextEditorStateChanged?.Invoke();
    }
	
	public void DisposeViewModel(
	    Key<TextEditorViewModel> viewModelKey)
	{
		lock (_stateModificationLock)
		{
			var inState = TextEditorState;

			var inViewModel = inState.ViewModelGetOrDefault(
				viewModelKey);

			if (inViewModel is null)
                goto finalize;

			inState._viewModelMap.Remove(inViewModel.ViewModelKey);
			inViewModel.Dispose();

            goto finalize;
        }

        finalize:
        TextEditorStateChanged?.Invoke();
    }
	
	public void SetViewModelWith(
	    ITextEditorEditContext editContext,
	    Key<TextEditorViewModel> viewModelKey,
	    Func<TextEditorViewModel, TextEditorViewModel> withFunc)
	{
		lock (_stateModificationLock)
		{
			var inState = TextEditorState;

			var inViewModel = inState.ViewModelGetOrDefault(
				viewModelKey);

			if (inViewModel is null)
                goto finalize;

			var outViewModel = withFunc.Invoke(inViewModel);
			inState._viewModelMap[inViewModel.ViewModelKey] = outViewModel;

            goto finalize;
        }

        finalize:
        TextEditorStateChanged?.Invoke();
    }
	
	public void SetModelAndViewModelRange(
	    ITextEditorEditContext editContext,
		Dictionary<ResourceUri, TextEditorModelModifier?>? modelModifierList,
		Dictionary<Key<TextEditorViewModel>, TextEditorViewModelModifier?>? viewModelModifierList)
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
		modelModifierList.
		
		I added 'if (null) continue;' sort of code but I will keep the 'try' 'catch'.
		*/

		lock (_stateModificationLock)
		{
			var inState = TextEditorState;

			try
			{
				// Models
				if (modelModifierList is not null)
				{
					foreach (var kvpModelModifier in modelModifierList)
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
				if (viewModelModifierList is not null)
				{
					foreach (var kvpViewModelModifier in viewModelModifierList)
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

            goto finalize;
        }

        finalize:
        TextEditorStateChanged?.Invoke();
    }
}
