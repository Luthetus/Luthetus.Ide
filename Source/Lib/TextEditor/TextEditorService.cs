using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Themes.States;
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
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Microsoft.JSInterop;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib;

public partial class TextEditorService : ITextEditorService
{
    /// <summary>
    /// See explanation of this field at: <see cref="TextEditorAuthenticatedAction"/>
    /// </summary>
    public static readonly Key<TextEditorAuthenticatedAction> AuthenticatedActionKey = new(Guid.Parse("13831968-9b10-46d1-8d47-842b78238d6a"));

	private readonly object _lockBackgroundTaskTryReusingSameInstance = new();

    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDispatcher _dispatcher;
    private readonly IDialogService _dialogService;
    private readonly LuthetusTextEditorConfig _textEditorOptions;
    private readonly ITextEditorRegistryWrap _textEditorRegistryWrap;
    private readonly IStorageService _storageService;
    // TODO: Perhaps do not reference IJSRuntime but instead wrap it in a 'IUiProvider' or something like that. The 'IUiProvider' would then expose methods that allow the TextEditorViewModel to adjust the scrollbars. 
    private readonly IJSRuntime _jsRuntime;
    private readonly LuthetusCommonBackgroundTaskApi _commonBackgroundTaskApi;

    public TextEditorService(
        IState<TextEditorModelState> modelStateWrap,
        IState<TextEditorViewModelState> viewModelStateWrap,
        IState<TextEditorGroupState> groupStateWrap,
        IState<TextEditorDiffState> diffStateWrap,
        IState<ThemeState> themeStateWrap,
        IState<TextEditorOptionsState> optionsStateWrap,
        IState<TextEditorFindAllState> findAllStateWrap,
        IBackgroundTaskService backgroundTaskService,
        LuthetusTextEditorConfig textEditorOptions,
        ITextEditorRegistryWrap textEditorRegistryWrap,
        IStorageService storageService,
        IJSRuntime jsRuntime,
        LuthetusCommonBackgroundTaskApi commonBackgroundTaskApi,
        IDispatcher dispatcher,
        IDialogService dialogService)
    {
        ModelStateWrap = modelStateWrap;
        ViewModelStateWrap = viewModelStateWrap;
        GroupStateWrap = groupStateWrap;
        DiffStateWrap = diffStateWrap;
        ThemeStateWrap = themeStateWrap;
        OptionsStateWrap = optionsStateWrap;
        FindAllStateWrap = findAllStateWrap;

        _backgroundTaskService = backgroundTaskService;
        _textEditorOptions = textEditorOptions;
        _textEditorRegistryWrap = textEditorRegistryWrap;
        _storageService = storageService;
        _jsRuntime = jsRuntime;
        _commonBackgroundTaskApi = commonBackgroundTaskApi;
        _dispatcher = dispatcher;
        _dialogService = dialogService;

        ModelApi = new TextEditorModelApi(this, _textEditorRegistryWrap.DecorationMapperRegistry, _textEditorRegistryWrap.CompilerServiceRegistry, _backgroundTaskService, _dispatcher);
        ViewModelApi = new TextEditorViewModelApi(this, _backgroundTaskService, ViewModelStateWrap, ModelStateWrap, _jsRuntime, _dispatcher, _dialogService);
        GroupApi = new TextEditorGroupApi(this, _dispatcher, _dialogService, _jsRuntime);
        DiffApi = new TextEditorDiffApi(this, _dispatcher);
        OptionsApi = new TextEditorOptionsApi(this, _textEditorOptions, _storageService, _commonBackgroundTaskApi, _dispatcher);
    }

	private TextEditorBackgroundTask? _backgroundTask;

    public IState<TextEditorModelState> ModelStateWrap { get; }
    public IState<TextEditorViewModelState> ViewModelStateWrap { get; }
    public IState<TextEditorGroupState> GroupStateWrap { get; }
    public IState<TextEditorDiffState> DiffStateWrap { get; }
    public IState<ThemeState> ThemeStateWrap { get; }
    public IState<TextEditorOptionsState> OptionsStateWrap { get; }
    public IState<TextEditorFindAllState> FindAllStateWrap { get; }

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

	public IEditContext OpenEditContext()
	{
		return new TextEditorEditContext(this, AuthenticatedActionKey);
	}

	public async Task CloseEditContext(IEditContext editContext)
	{
		foreach (var modelModifier in editContext.ModelCache.Values)
		{
			if (modelModifier is null || !modelModifier.WasModified)
				continue;

			_dispatcher.Dispatch(new TextEditorModelState.SetAction(
				editContext.AuthenticatedActionKey,
				editContext,
				modelModifier));

			var viewModelBag = ViewModelStateWrap.Value.ViewModelList.Where(
				x => x.ResourceUri == modelModifier.ResourceUri);

			foreach (var viewModel in viewModelBag)
			{
				// Invoking 'GetViewModelModifier' marks the view model to be updated.
				editContext.GetViewModelModifier(viewModel.ViewModelKey);
			}

			if (modelModifier.WasDirty != modelModifier.IsDirty)
			{
				if (modelModifier.IsDirty)
					_dispatcher.Dispatch(new DirtyResourceUriState.AddDirtyResourceUriAction(
						modelModifier.ResourceUri));
				else
					_dispatcher.Dispatch(new DirtyResourceUriState.RemoveDirtyResourceUriAction(
						modelModifier.ResourceUri));
			}
		}

		foreach (var viewModelModifier in editContext.ViewModelCache.Values)
		{
			if (viewModelModifier is null || !viewModelModifier.WasModified)
				return;

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
				await ((TextEditorService)editContext.TextEditorService)
					.HACK_SetScrollPosition(viewModelModifier.ViewModel)
					.ConfigureAwait(false);
			}

			// TODO: This 'CalculateVirtualizationResultFactory' invocation is horrible for performance.
			await editContext.TextEditorService.ViewModelApi.CalculateVirtualizationResultFactory(
					viewModelModifier.ViewModel.ResourceUri,
					viewModelModifier.ViewModel.ViewModelKey,
					CancellationToken.None)
				.Invoke(editContext)
				.ConfigureAwait(false);

			_dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
				editContext.AuthenticatedActionKey,
				editContext,
				viewModelModifier.ViewModel.ViewModelKey,
				inState => viewModelModifier.ViewModel));
		}
	}

	public Task Post(ITextEditorWork work)
    {
        lock (_lockBackgroundTaskTryReusingSameInstance)
		{
			if (_backgroundTask is null || !_backgroundTask.TryReusingSameInstance(work))
				_backgroundTask = new(this, work);
		}

		return _backgroundTaskService.EnqueueAsync(_backgroundTask);
    }

    public Task Post(
		ResourceUri resourceUri,
		Key<TextEditorCursor> cursorKey,
		Func<IEditContext, Key<TextEditorCursor>, TextEditorCursor> getCursorFunc,
		TextEditorEdit edit)
	{
		throw new NotImplementedException();
	}

	public Task Post(
		ResourceUri resourceUri,
		Key<TextEditorViewModel> viewModelKey,
		TextEditorEdit edit)
	{
		throw new NotImplementedException();
	}

    /// <summary>
    /// I want to batch any scrolling done while within an <see cref="IEditContext"/>.
    /// The <see cref="TextEditorServiceTask"/> needs the <see cref="IJSRuntime"/>,
    /// in order to perform the scroll once the <see cref="IEditContext"/> is completed.
    /// That being said, I didn't want to pass the <see cref="IJSRuntime"/> to the <see cref="TextEditorServiceTask"/>
    /// so I'm doing this for the moment (2024-05-09).
    /// </summary>
    public async Task HACK_SetScrollPosition(TextEditorViewModel viewModel)
    {
        await _jsRuntime.GetLuthetusTextEditorApi()
            .SetScrollPosition(
                viewModel.BodyElementId,
                viewModel.GutterElementId,
                viewModel.VirtualizationResult.TextEditorMeasurements.ScrollLeft,
                viewModel.VirtualizationResult.TextEditorMeasurements.ScrollTop)
            .ConfigureAwait(false);
    }

    public record TextEditorEditContext : IEditContext
    {
        public Dictionary<ResourceUri, TextEditorModelModifier?> ModelCache { get; } = new();
        public Dictionary<Key<TextEditorViewModel>, ResourceUri?> ViewModelToModelResourceUriCache { get; } = new();
        public Dictionary<Key<TextEditorViewModel>, TextEditorViewModelModifier?> ViewModelCache { get; } = new();
        public Dictionary<Key<TextEditorViewModel>, CursorModifierBagTextEditor?> CursorModifierBagCache { get; } = new();
        public Dictionary<Key<TextEditorCursor>, TextEditorCursorModifier?> CursorModifierCache { get; } = new();
        public Dictionary<Key<TextEditorDiffModel>, TextEditorDiffModelModifier?> DiffModelCache { get; } = new();

        public TextEditorEditContext(
            ITextEditorService textEditorService,
            Key<TextEditorAuthenticatedAction> authenticatedActionKey)
        {
            TextEditorService = textEditorService;
            AuthenticatedActionKey = authenticatedActionKey;
        }

        public ITextEditorService TextEditorService { get; }
        public Key<TextEditorAuthenticatedAction> AuthenticatedActionKey { get; }

        public TextEditorModelModifier? GetModelModifier(
            ResourceUri? modelResourceUri,
            bool isReadonly = false)
        {
            if (modelResourceUri is not null)
            {
                if (!ModelCache.TryGetValue(modelResourceUri, out var modelModifier))
                {
					var model = TextEditorService.ModelStateWrap.Value.ModelList.FirstOrDefault(
						x => x.ResourceUri == modelResourceUri);

                    modelModifier = model is null ? null : new(model);

                    ModelCache.Add(modelResourceUri, modelModifier);
                }

                if (!isReadonly && modelModifier is not null)
                    modelModifier.WasModified = true;

                return modelModifier;
            }

            return null;
        }

        public TextEditorModelModifier? GetModelModifierByViewModelKey(
            Key<TextEditorViewModel> viewModelKey,
            bool isReadonly = false)
        {
            if (viewModelKey != Key<TextEditorViewModel>.Empty)
            {
                if (!ViewModelToModelResourceUriCache.TryGetValue(viewModelKey, out var modelResourceUri))
                {
                    var model = TextEditorService.ViewModelApi.GetModelOrDefault(viewModelKey);
                    modelResourceUri = model?.ResourceUri;

                    ViewModelToModelResourceUriCache.Add(viewModelKey, modelResourceUri);
                }

                return GetModelModifier(modelResourceUri);
            }

            return null;
        }

        public TextEditorViewModelModifier? GetViewModelModifier(
            Key<TextEditorViewModel> viewModelKey,
            bool isReadonly = false)
        {
            if (viewModelKey != Key<TextEditorViewModel>.Empty)
            {
                if (!ViewModelCache.TryGetValue(viewModelKey, out var viewModelModifier))
                {
                    var viewModel = TextEditorService.ViewModelApi.GetOrDefault(viewModelKey);
                    viewModelModifier = viewModel is null ? null : new(viewModel);

                    ViewModelCache.Add(viewModelKey, viewModelModifier);
                }

                if (!isReadonly && viewModelModifier is not null)
                    viewModelModifier.WasModified = true;

                return viewModelModifier;
            }

            return null;
        }

        public CursorModifierBagTextEditor? GetCursorModifierBag(TextEditorViewModel? viewModel)
        {
            if (viewModel is not null)
            {
                if (!CursorModifierBagCache.TryGetValue(viewModel.ViewModelKey, out var cursorModifierBag))
                {
                    cursorModifierBag = new CursorModifierBagTextEditor(
                        viewModel.ViewModelKey,
                        viewModel.CursorList.Select(x => new TextEditorCursorModifier(x)).ToList());

                    CursorModifierBagCache.Add(viewModel.ViewModelKey, cursorModifierBag);
                }

                return cursorModifierBag;
            }

            return null;
        }

        public TextEditorCursorModifier? GetPrimaryCursorModifier(CursorModifierBagTextEditor? cursorModifierBag)
        {
            var primaryCursor = (TextEditorCursorModifier?)null;

            if (cursorModifierBag is not null)
                primaryCursor = cursorModifierBag.List.FirstOrDefault(x => x.IsPrimaryCursor);

            return primaryCursor;
        }

		public TextEditorCursorModifier? GetCursorModifier(
			Key<TextEditorCursor> cursorKey,
			Func<IEditContext, Key<TextEditorCursor>, TextEditorCursor> getCursorFunc)
		{
			Console.WriteLine(cursorKey);
			if (cursorKey != Key<TextEditorCursor>.Empty)
			{
				if (!CursorModifierCache.TryGetValue(cursorKey, out var cursorModifier))
				{
					cursorModifier = new TextEditorCursorModifier(getCursorFunc.Invoke(this, cursorKey));
					CursorModifierCache.Add(cursorKey, cursorModifier);
				}

				return cursorModifier;
			}

			return null;
		}

        public TextEditorDiffModelModifier? GetDiffModelModifier(
            Key<TextEditorDiffModel> diffModelKey,
            bool isReadonly = false)
        {
            if (diffModelKey != Key<TextEditorDiffModel>.Empty)
            {
                if (!DiffModelCache.TryGetValue(diffModelKey, out var diffModelModifier))
                {
                    var diffModel = TextEditorService.DiffApi.GetOrDefault(diffModelKey);
                    diffModelModifier = diffModel is null ? null : new(diffModel);

                    DiffModelCache.Add(diffModelKey, diffModelModifier);
                }

                if (!isReadonly && diffModelModifier is not null)
                    diffModelModifier.WasModified = true;

                return diffModelModifier;
            }

            return null;
        }
    }
}
