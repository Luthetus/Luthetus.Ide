using System.Text;
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
using Luthetus.TextEditor.RazorLib.Lines.Models;
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
using Luthetus.TextEditor.RazorLib.Cursors.Models;

namespace Luthetus.TextEditor.RazorLib;

public sealed class TextEditorService
{
    private readonly BackgroundTaskService _backgroundTaskService;
    private readonly IPanelService _panelService;
    private readonly IDialogService _dialogService;
    private readonly IDirtyResourceUriService _dirtyResourceUriService;
    private readonly ITextEditorRegistryWrap _textEditorRegistryWrap;
    private readonly IStorageService _storageService;
    private readonly IJSRuntime _jsRuntime;
    private readonly CommonBackgroundTaskApi _commonBackgroundTaskApi;
    private readonly IServiceProvider _serviceProvider;

    public TextEditorService(
        IFindAllService findAllService,
        IDirtyResourceUriService dirtyResourceUriService,
        IThemeService themeService,
        BackgroundTaskService backgroundTaskService,
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
    	__TextEditorViewModelLiason = new(this);
    
    	WorkerUi = new(this);
    	WorkerArbitrary = new(this);
    
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
        _commonBackgroundTaskApi = commonBackgroundTaskApi;
        _dialogService = dialogService;

		AutocompleteIndexer = autocompleteIndexer;
		AutocompleteService = autocompleteService;

        ModelApi = new TextEditorModelApi(this, _textEditorRegistryWrap, _backgroundTaskService);
        ViewModelApi = new TextEditorViewModelApi(this, _backgroundTaskService, _commonBackgroundTaskApi, _dialogService, _panelService);
        GroupApi = new TextEditorGroupApi(this, _panelService, _dialogService, _commonBackgroundTaskApi);
        DiffApi = new TextEditorDiffApi(this);
        OptionsApi = new TextEditorOptionsApi(this, TextEditorConfig, _storageService, _dialogService, contextService, themeService, _commonBackgroundTaskApi);
        
        TextEditorState = new();
    }

    public IThemeService ThemeService { get; }
    public IAppDimensionService AppDimensionService { get; }
    public IFindAllService FindAllService { get; }

	public LuthetusTextEditorJavaScriptInteropApi JsRuntimeTextEditorApi { get; }
	public LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi => _commonBackgroundTaskApi.JsRuntimeCommonApi;
	public IAutocompleteIndexer AutocompleteIndexer { get; }
	public IAutocompleteService AutocompleteService { get; }
	public LuthetusTextEditorConfig TextEditorConfig { get; }

#if DEBUG
    public string StorageKey => "luth_te_text-editor-options-debug";
#else
    public string StorageKey => "luth_te_text-editor-options";
#endif

    public string ThemeCssClassString { get; set; }

    public TextEditorModelApi ModelApi { get; }
    public TextEditorViewModelApi ViewModelApi { get; }
    public TextEditorGroupApi GroupApi { get; }
    public TextEditorDiffApi DiffApi { get; }
    public TextEditorOptionsApi OptionsApi { get; }
    
    public TextEditorState TextEditorState { get; }
    
    public TextEditorWorkerUi WorkerUi { get; }
	public TextEditorWorkerArbitrary WorkerArbitrary { get; }
    
    public BackgroundTaskService BackgroundTaskService => _backgroundTaskService;
    
    /// <summary>
	/// Do not touch this property, it is used for the VirtualizationGrid.
	/// </summary>
	public StringBuilder __StringBuilder { get; } = new StringBuilder();
	
    /// <summary>
	/// Do not touch this property, it is used for the TextEditorEditContext.
	/// </summary>
    public Dictionary<Key<TextEditorDiffModel>, TextEditorDiffModelModifier?> __DiffModelCache { get; } = new();
 
	/// <summary>
	/// Do not touch this property, it is used for the TextEditorEditContext.
	/// </summary>
	public List<TextEditorModel> __ModelList { get; } = new();   
    /// <summary>
	/// Do not touch this property, it is used for the TextEditorEditContext.
	/// </summary>
    public List<TextEditorViewModel> __ViewModelList { get; } = new();
    
    /// <summary>
	/// Do not touch this property, it is used for the 'TextEditorModel.InsertMetadata(...)' method.
	/// </summary>
    public List<LineEnd> __LocalLineEndList { get; } = new();
    /// <summary>
	/// Do not touch this property, it is used for the 'TextEditorModel.InsertMetadata(...)' method.
	/// </summary>
    public List<int> __LocalTabPositionList { get; } = new();
    /// <summary>
	/// Do not touch this property, it is used for the 'TextEditorModel.InsertMetadata(...)' method.
	/// </summary>
    public TextEditorViewModelLiason __TextEditorViewModelLiason { get; }
    
    public event Action? TextEditorStateChanged;

	public async ValueTask FinalizePost(TextEditorEditContext editContext)
	{
        for (int modelIndex = 0; modelIndex < __ModelList.Count; modelIndex++)
        {
        	var modelModifier = __ModelList[modelIndex];
        	
            for (int viewModelIndex = 0; viewModelIndex < modelModifier.PersistentState.ViewModelKeyList.Count; viewModelIndex++)
            {
                // Invoking 'GetViewModelModifier' marks the view model to be updated.
                var viewModelModifier = editContext.GetViewModelModifier(modelModifier.PersistentState.ViewModelKeyList[viewModelIndex]);

				if (!viewModelModifier.ShouldCalculateVirtualizationResult)
					viewModelModifier.ShouldCalculateVirtualizationResult = modelModifier.ShouldCalculateVirtualizationResult;
            }

            if (modelModifier.WasDirty != modelModifier.IsDirty)
            {
            	var model = ModelApi.GetOrDefault(modelModifier.PersistentState.ResourceUri);
            	model.IsDirty = modelModifier.IsDirty;
            
                if (modelModifier.IsDirty)
                    _dirtyResourceUriService.AddDirtyResourceUri(modelModifier.PersistentState.ResourceUri);
                else
                    _dirtyResourceUriService.RemoveDirtyResourceUri(modelModifier.PersistentState.ResourceUri);
            }
            
            if (TextEditorState._modelMap.ContainsKey(modelModifier.PersistentState.ResourceUri))
				TextEditorState._modelMap[modelModifier.PersistentState.ResourceUri] = modelModifier;
        }
		
        for (int viewModelIndex = 0; viewModelIndex < __ViewModelList.Count; viewModelIndex++)
        {
        	var viewModelModifier = __ViewModelList[viewModelIndex];
        
        	TextEditorModel? modelModifier = null;
        	if (viewModelModifier.PersistentState.ShouldRevealCursor || viewModelModifier.ShouldCalculateVirtualizationResult || viewModelModifier.ScrollWasModified)
        		modelModifier = editContext.GetModelModifier(viewModelModifier.PersistentState.ResourceUri, isReadOnly: true);
        
        	if (viewModelModifier.PersistentState.ShouldRevealCursor)
            {
        		ViewModelApi.RevealCursor(
            		editContext,
			        modelModifier,
			        viewModelModifier);
            }
            
            // This if expression exists below, to check if 'CalculateVirtualizationResult(...)' should be invoked.
            //
            // But, note that these cannot be combined at the bottom, we need to check if an edit
            // reduced the scrollWidth or scrollHeight of the editor's content.
            // 
            // This is done here, so that the 'ScrollWasModified' bool can be set, and downstream if statements will be entered,
            // which go on to scroll the editor.
            if (viewModelModifier.ShouldCalculateVirtualizationResult)
			{
				ValidateMaximumScrollLeftAndScrollTop(editContext, modelModifier, viewModelModifier, textEditorDimensionsChanged: false);
			}

            if (!viewModelModifier.ShouldCalculateVirtualizationResult &&
            	viewModelModifier.ScrollWasModified)
            {
            	// If not already going to reload virtualization result,
            	// then check if the virtualization needs to be refreshed due to a
            	// change in scroll position.
            	//
            	// This code only needs to run if the scrollbar was modified.
            	
            	if (viewModelModifier.VirtualizationResult.EntryList.Count > 0)
            	{
            		if (viewModelModifier.ScrollTop < viewModelModifier.VirtualizationResult.VirtualTop)
            		{
            			viewModelModifier.ShouldCalculateVirtualizationResult = true;
            		}
            		else
            		{
            			var bigTop = viewModelModifier.ScrollTop + viewModelModifier.TextEditorDimensions.Height;
            			var virtualEnd = viewModelModifier.VirtualizationResult.VirtualTop + viewModelModifier.VirtualizationResult.VirtualHeight;
            				
            			if (bigTop > virtualEnd)
            				viewModelModifier.ShouldCalculateVirtualizationResult = true;
            		}
            	}
            	
            	// A check for horizontal virtualization still needs to be done.
            	//
            	// If we didn't already determine the necessity of calculating the virtualization
            	// result when checking the vertical virtualization, then we check horizontal.
            	if (!viewModelModifier.ShouldCalculateVirtualizationResult)
            	{
            		var scrollLeft = viewModelModifier.ScrollLeft;
            		if (scrollLeft < (viewModelModifier.VirtualizationResult.VirtualLeft))
            		{
            			viewModelModifier.ShouldCalculateVirtualizationResult = true;
            		}
            		else
            		{
						var bigLeft = scrollLeft + viewModelModifier.TextEditorDimensions.Width;
            			if (bigLeft > viewModelModifier.VirtualizationResult.VirtualLeft + viewModelModifier.VirtualizationResult.VirtualWidth)
            				viewModelModifier.ShouldCalculateVirtualizationResult = true;
            		}
            	}
            }

			if (viewModelModifier.ShouldCalculateVirtualizationResult)
			{
				var componentData = viewModelModifier.PersistentState.DisplayTracker.ComponentData;
				
				if (componentData is not null)
				{
					// TODO: This 'CalculateVirtualizationResultFactory' invocation is horrible for performance.
		            editContext.TextEditorService.ViewModelApi.CalculateVirtualizationResult(
		            	editContext,
		            	modelModifier,
				        viewModelModifier,
				        componentData);
				}
			}
			
			if (TextEditorState._viewModelMap.ContainsKey(viewModelModifier.PersistentState.ViewModelKey))
				TextEditorState._viewModelMap[viewModelModifier.PersistentState.ViewModelKey] = viewModelModifier;
        }
	    
	    // __DiffModelCache.Clear();
	    
	    __ModelList.Clear();
		__ViewModelList.Clear();

        TextEditorStateChanged?.Invoke();
	    
	    // SetModelAndViewModelRange(editContext);
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
		TextEditorEditContext editContext,
		TextEditorModel? modelModifier,
		TextEditorViewModel viewModelModifier,
		bool textEditorDimensionsChanged)
	{
    	if (modelModifier is null)
    		return;
		
		var originalScrollWidth = viewModelModifier.ScrollWidth;
		var originalScrollHeight = viewModelModifier.ScrollHeight;
	
		var totalWidth = (int)Math.Ceiling(modelModifier.MostCharactersOnASingleLineTuple.lineLength *
			viewModelModifier.CharAndLineMeasurements.CharacterWidth);

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
				viewModelModifier.CharAndLineMeasurements.CharacterWidth);
		}

		var totalHeight = (int)Math.Ceiling(
			(modelModifier.LineEndList.Count - viewModelModifier.HiddenLineIndexHashSet.Count) *
			viewModelModifier.CharAndLineMeasurements.LineHeight);

		// Add vertical margin so the user can scroll beyond the final line of content
		int marginScrollHeight;
		{
			var percentOfMarginScrollHeightByPageUnit = 0.4;

			marginScrollHeight = (int)Math.Ceiling(viewModelModifier.TextEditorDimensions.Height * percentOfMarginScrollHeightByPageUnit);
			totalHeight += marginScrollHeight;
		}

		viewModelModifier.ScrollWidth = totalWidth;
		viewModelModifier.ScrollHeight = totalHeight;
		viewModelModifier.MarginScrollHeight = marginScrollHeight;
		
		// var validateScrollWidth = totalWidth;
		// var validateScrollHeight = totalHeight;
		// var validateMarginScrollHeight = marginScrollHeight;
		
		if (originalScrollWidth > viewModelModifier.ScrollWidth ||
			textEditorDimensionsChanged)
		{
			viewModelModifier.SetScrollLeft(
				(int)viewModelModifier.ScrollLeft,
				viewModelModifier.TextEditorDimensions);
		}
		
		if (originalScrollHeight > viewModelModifier.ScrollHeight ||
			textEditorDimensionsChanged)
		{
			viewModelModifier.SetScrollTop(
				(int)viewModelModifier.ScrollTop,
				viewModelModifier.TextEditorDimensions);
			
			// The scrollLeft currently does not have any margin. Therefore subtracting the margin isn't needed.
			//
			// For scrollTop however, if one does not subtract the MarginScrollHeight in the case of
			// 'textEditorDimensionsChanged'
			//
			// Then a "void" will render at the top portion of the text editor, seemingly the size
			// of the MarginScrollHeight.
			if (textEditorDimensionsChanged &&
				viewModelModifier.ScrollTop != viewModelModifier.ScrollTop)
			{
				viewModelModifier.SetScrollTop(
					(int)viewModelModifier.ScrollTop - (int)viewModelModifier.MarginScrollHeight,
					viewModelModifier.TextEditorDimensions);
			}
		}
		
		var changeOccurred =
			viewModelModifier.ScrollLeft != viewModelModifier.ScrollLeft ||
			viewModelModifier.ScrollTop != viewModelModifier.ScrollTop;
		
		if (changeOccurred)
		{
			viewModelModifier.ScrollWasModified = true;
		}
	}
	
	public async Task OpenInEditorAsync(
		TextEditorEditContext editContext,
		string absolutePath,
		bool shouldSetFocusToEditor,
		int? cursorPositionIndex,
		Category category,
		Key<TextEditorViewModel> preferredViewModelKey)
	{
		var resourceUri = new ResourceUri(absolutePath);
		var actualViewModelKey = await CommonLogic_OpenInEditorAsync(
			editContext,
			resourceUri,
			shouldSetFocusToEditor,
			category,
			preferredViewModelKey);
			
		// Move cursor
		if (cursorPositionIndex is null)
			return; // Leave the cursor unchanged if the argument is null
		var modelModifier = editContext.GetModelModifier(resourceUri);
		var viewModelModifier = editContext.GetViewModelModifier(actualViewModelKey);

		if (modelModifier is null || viewModelModifier is null)
			return;
	
		var lineAndColumnIndices = modelModifier.GetLineAndColumnIndicesFromPositionIndex(cursorPositionIndex.Value);
			
		viewModelModifier.LineIndex = lineAndColumnIndices.lineIndex;
		viewModelModifier.ColumnIndex = lineAndColumnIndices.columnIndex;
		
		viewModelModifier.PersistentState.ShouldRevealCursor = true;
		
		_ = Task.Run(async () =>
		{
			await Task.Delay(200).ConfigureAwait(false);
			WorkerArbitrary.PostUnique(nameof(OpenInEditorAsync), editContext =>
			{
				var viewModelModifier = editContext.GetViewModelModifier(actualViewModelKey);
				viewModelModifier.PersistentState.ShouldRevealCursor = true;
				return ValueTask.CompletedTask;
			});
		});
	}
	
	public async Task OpenInEditorAsync(
		TextEditorEditContext editContext,
		string absolutePath,
		bool shouldSetFocusToEditor,
		int? lineIndex,
		int? columnIndex,
		Category category,
		Key<TextEditorViewModel> preferredViewModelKey)
	{
		// Standardize Resource Uri
		if (TextEditorConfig.AbsolutePathStandardizeFunc is null)
			return;
			
		var standardizedFilePathString = await TextEditorConfig.AbsolutePathStandardizeFunc
			.Invoke(absolutePath, _serviceProvider)
			.ConfigureAwait(false);
			
		var resourceUri = new ResourceUri(standardizedFilePathString);

		var actualViewModelKey = await CommonLogic_OpenInEditorAsync(
			editContext,
			resourceUri,
			shouldSetFocusToEditor,
			category,
			preferredViewModelKey);
			
		// Move cursor
		if (lineIndex is null && columnIndex is null)
			return; // Leave the cursor unchanged if the argument is null
		var modelModifier = editContext.GetModelModifier(resourceUri);
		var viewModelModifier = editContext.GetViewModelModifier(actualViewModelKey);

		if (modelModifier is null || viewModelModifier is null)
			return;
	
		if (lineIndex is not null)
			viewModelModifier.LineIndex = lineIndex.Value;
		if (columnIndex is not null)
			viewModelModifier.ColumnIndex = columnIndex.Value;
		
		if (viewModelModifier.LineIndex > modelModifier.LineCount - 1)
			viewModelModifier.LineIndex = modelModifier.LineCount - 1;
		
		var lineInformation = modelModifier.GetLineInformation(viewModelModifier.LineIndex);
		
		if (viewModelModifier.ColumnIndex > lineInformation.LastValidColumnIndex)
			viewModelModifier.SetColumnIndexAndPreferred(lineInformation.LastValidColumnIndex);
			
		viewModelModifier.PersistentState.ShouldRevealCursor = true;
		
		_ = Task.Run(async () =>
		{
			await Task.Delay(200).ConfigureAwait(false);
			WorkerArbitrary.PostUnique(nameof(OpenInEditorAsync), editContext =>
			{
				var viewModelModifier = editContext.GetViewModelModifier(actualViewModelKey);
				viewModelModifier.PersistentState.ShouldRevealCursor = true;
				return ValueTask.CompletedTask;
			});
		});
	}
	
	/// <summary>
	/// Returns Key<TextEditorViewModel>.Empty if it failed to open in editor.
	/// Returns the ViewModel's key (non Key<TextEditorViewModel>.Empty value) if it successfully opened in editor.
	/// </summary>
	private async Task<Key<TextEditorViewModel>> CommonLogic_OpenInEditorAsync(
		TextEditorEditContext editContext,
		ResourceUri resourceUri,
		bool shouldSetFocusToEditor,
		Category category,
		Key<TextEditorViewModel> preferredViewModelKey)
	{
		// RegisterModelFunc
		if (TextEditorConfig.RegisterModelFunc is null)
			return Key<TextEditorViewModel>.Empty;
		await TextEditorConfig.RegisterModelFunc
			.Invoke(new RegisterModelArgs(editContext, resourceUri, _serviceProvider))
			.ConfigureAwait(false);
	
		// TryRegisterViewModelFunc
		if (TextEditorConfig.TryRegisterViewModelFunc is null)
			return Key<TextEditorViewModel>.Empty;
		var actualViewModelKey = await TextEditorConfig.TryRegisterViewModelFunc
			.Invoke(new TryRegisterViewModelArgs(editContext, preferredViewModelKey, resourceUri, category, shouldSetFocusToEditor, _serviceProvider))
			.ConfigureAwait(false);
	
		// TryShowViewModelFunc
		if (actualViewModelKey == Key<TextEditorViewModel>.Empty || TextEditorConfig.TryShowViewModelFunc is null)
			return Key<TextEditorViewModel>.Empty;
		await TextEditorConfig.TryShowViewModelFunc
			.Invoke(new TryShowViewModelArgs(actualViewModelKey, Key<TextEditorGroup>.Empty, shouldSetFocusToEditor, _serviceProvider))
			.ConfigureAwait(false);
		
		return actualViewModelKey;
	}
	
	// Move TextEditorState.Reducer.cs here
	public void RegisterModel(TextEditorEditContext editContext, TextEditorModel model)
	{
	    var inState = TextEditorState;
	
	    var exists = inState._modelMap.TryGetValue(
	        model.PersistentState.ResourceUri,
	        out _);
	
	    if (exists)
	        return;
	
	    inState._modelMap.Add(model.PersistentState.ResourceUri, model);
	
	    TextEditorStateChanged?.Invoke();
	}

	public void DisposeModel(TextEditorEditContext editContext, ResourceUri resourceUri)
	{
	    var inState = TextEditorState;
	
	    var exists = inState._modelMap.TryGetValue(
	        resourceUri,
	        out var model);
	
	    if (!exists)
	        return;
	        
	    foreach (var viewModelKey in model.PersistentState.ViewModelKeyList)
	    {
	        DisposeViewModel(editContext, viewModelKey);
	    }
	
	    inState._modelMap.Remove(resourceUri);
	
	    TextEditorStateChanged?.Invoke();
	}
	
	public void SetModel(
	    TextEditorEditContext editContext,
	    TextEditorModel modelModifier)
	{
		var inState = TextEditorState;

		var exists = inState._modelMap.TryGetValue(
			modelModifier.PersistentState.ResourceUri, out var inModel);

		if (!exists)
            return;

		inState._modelMap[inModel.PersistentState.ResourceUri] = modelModifier;

        TextEditorStateChanged?.Invoke();
    }
	
	public void RegisterViewModel(TextEditorEditContext editContext, TextEditorViewModel viewModel)
	{
	    var inState = TextEditorState;
	
	    var modelExisting = inState._modelMap.TryGetValue(
	        viewModel.PersistentState.ResourceUri,
	        out var model);
	
	    if (!modelExisting)
	        return;
	
	    if (viewModel.PersistentState.ViewModelKey == Key<TextEditorViewModel>.Empty)
	        throw new InvalidOperationException($"Provided {nameof(Key<TextEditorViewModel>)} cannot be {nameof(Key<TextEditorViewModel>)}.{Key<TextEditorViewModel>.Empty}");
	
	    var viewModelExisting = inState.ViewModelGetOrDefault(viewModel.PersistentState.ViewModelKey);
	    if (viewModelExisting is not null)
	        return;
	
	    model.PersistentState.ViewModelKeyList.Add(viewModel.PersistentState.ViewModelKey);
	
	    inState._viewModelMap.Add(viewModel.PersistentState.ViewModelKey, viewModel);
	
	    TextEditorStateChanged?.Invoke();
	}
	
	public void DisposeViewModel(TextEditorEditContext editContext, Key<TextEditorViewModel> viewModelKey)
	{
	    var inState = TextEditorState;
	    
	    var viewModel = inState.ViewModelGetOrDefault(viewModelKey);
	    if (viewModel is null)
	        return;
	    
	    inState._viewModelMap.Remove(viewModel.PersistentState.ViewModelKey);
	    viewModel.Dispose();
	
	    var model = inState.ModelGetOrDefault(viewModel.PersistentState.ResourceUri);
	    if (model is not null)
	        model.PersistentState.ViewModelKeyList.Remove(viewModel.PersistentState.ViewModelKey);
	    
	    TextEditorStateChanged?.Invoke();
	}
	
	public void SetModelAndViewModelRange(TextEditorEditContext editContext)
	{
		// TextEditorState isn't currently being re-instantiated after the state is modified, so I'm going to comment out this local reference.
		// 
		// var inState = TextEditorState;

		if (__ModelList.Count > 0)
		{
			foreach (var model in __ModelList)
			{
				if (TextEditorState._modelMap.ContainsKey(model.PersistentState.ResourceUri))
					TextEditorState._modelMap[model.PersistentState.ResourceUri] = model;
			}
			
			__ModelList.Clear();
		}
		
		if (__ViewModelList.Count > 0)
		{
			foreach (var viewModel in __ViewModelList)
			{
				if (TextEditorState._viewModelMap.ContainsKey(viewModel.PersistentState.ViewModelKey))
					TextEditorState._viewModelMap[viewModel.PersistentState.ViewModelKey] = viewModel;
			}
			
			__ViewModelList.Clear();
		}

        TextEditorStateChanged?.Invoke();
    }
}
