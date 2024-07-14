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
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
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
		IState<AppDimensionState> appDimensionStateWrap)
    {
        TextEditorStateWrap = textEditorStateWrap;
        GroupStateWrap = groupStateWrap;
        DiffStateWrap = diffStateWrap;
        ThemeStateWrap = themeStateWrap;
        OptionsStateWrap = optionsStateWrap;
        FindAllStateWrap = findAllStateWrap;
		AppDimensionStateWrap = appDimensionStateWrap;

        _backgroundTaskService = backgroundTaskService;
        TextEditorConfig = textEditorConfig;
        _textEditorRegistryWrap = textEditorRegistryWrap;
        _storageService = storageService;
        _jsRuntime = jsRuntime;
		JsRuntimeTextEditorApi = _jsRuntime.GetLuthetusTextEditorApi();
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

    public void PostSimpleBatch(
        string name,
        TextEditorEdit textEditorEdit,
        TimeSpan? throttleTimeSpan = null)
    {
        Post(new SimpleBatchTextEditorTask(
            name,
            textEditorEdit,
            throttleTimeSpan));
    }

    public void PostTakeMostRecent(
        string name,
		ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorEdit textEditorEdit,
        TimeSpan? throttleTimeSpan = null)
    {
        Post(new TakeMostRecentTextEditorTask(
            name,
			resourceUri,
            viewModelKey,
            textEditorEdit,
            throttleTimeSpan));
    }

    public void Post(ITextEditorTask task)
    {
        task.EditContext = new TextEditorEditContext(
            this,
            AuthenticatedActionKey);

        _backgroundTaskService.Enqueue(task);
    }

	public async Task FinalizePost(IEditContext editContext)
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

			if (viewModelModifier.ShouldReloadVirtualizationResult)
			{
				// TODO: This 'CalculateVirtualizationResultFactory' invocation is horrible for performance.
	            await editContext.TextEditorService.ViewModelApi.CalculateVirtualizationResultFactory(
	                    viewModelModifier.ViewModel.ResourceUri, viewModelModifier.ViewModel.ViewModelKey, CancellationToken.None)
	                .Invoke(editContext)
	                .ConfigureAwait(false);
			}
            
			_dispatcher.Dispatch(new TextEditorState.SetModelAndViewModelRangeAction(
                editContext.AuthenticatedActionKey,
                editContext,
                modelModifierNeedRenderList,
				viewModelModifierNeedRenderList));
        }
	}
}
