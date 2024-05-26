using Fluxor;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
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

    public Task PostSimpleBatch(
        string name,
        string identifier,
        TextEditorEdit textEditorEdit,
        TimeSpan? throttleTimeSpan = null)
    {
        return Post(new SimpleBatchTextEditorTask(
            $"{name}_sb",
			identifier,
            textEditorEdit,
            throttleTimeSpan));
    }

    public Task PostTakeMostRecent(
        string name,
        string redundancyIdentifier,
        TextEditorEdit textEditorEdit,
        TimeSpan? throttleTimeSpan = null)
    {
        return Post(new TakeMostRecentTextEditorTask(
            $"{name}_tmr",
            redundancyIdentifier,
            textEditorEdit,
            throttleTimeSpan));
    }

    public async Task Post(ITextEditorTask innerTask)
    {
        try
        {
            var editContext = new TextEditorEditContext(
                this,
                AuthenticatedActionKey);

            var textEditorServiceTask = new TextEditorServiceTask(
                innerTask,
                editContext,
                _dispatcher);

            await _backgroundTaskService.EnqueueAsync(textEditorServiceTask).ConfigureAwait(false);
        }
        catch (LuthetusTextEditorException e)
        {
            Console.WriteLine(e.ToString());
        }
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

    private record TextEditorEditContext : IEditContext
    {
        public Dictionary<ResourceUri, TextEditorModelModifier?> ModelCache { get; } = new();
        public Dictionary<Key<TextEditorViewModel>, ResourceUri?> ViewModelToModelResourceUriCache { get; } = new();
        public Dictionary<Key<TextEditorViewModel>, TextEditorViewModelModifier?> ViewModelCache { get; } = new();
        public Dictionary<Key<TextEditorViewModel>, CursorModifierBagTextEditor?> CursorModifierBagCache { get; } = new();
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
                    var model = TextEditorService.ModelApi.GetOrDefault(modelResourceUri);
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
