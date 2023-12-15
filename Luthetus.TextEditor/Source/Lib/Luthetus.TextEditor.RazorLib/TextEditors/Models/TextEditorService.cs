using Fluxor;
using Luthetus.Common.RazorLib.Storages.States;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.RazorLib.SearchEngines.States;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Microsoft.JSInterop;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using static Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices.ITextEditorService;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Commands.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial class TextEditorService : ITextEditorService
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDispatcher _dispatcher;
    private readonly LuthetusTextEditorOptions _textEditorOptions;
    private readonly ITextEditorRegistryWrap _textEditorRegistryWrap;
    private readonly IStorageService _storageService;
    // TODO: Perhaps do not reference IJSRuntime but instead wrap it in a 'IUiProvider' or something like that. The 'IUiProvider' would then expose methods that allow the TextEditorViewModel to adjust the scrollbars. 
    private readonly IJSRuntime _jsRuntime;
    private readonly StorageSync _storageSync;

    public TextEditorService(
        IState<TextEditorModelState> modelStateWrap,
        IState<TextEditorViewModelState> viewModelStateWrap,
        IState<TextEditorGroupState> groupStateWrap,
        IState<TextEditorDiffState> diffStateWrap,
        IState<ThemeState> themeStateWrap,
        IState<TextEditorOptionsState> optionsStateWrap,
        IState<TextEditorSearchEngineState> searchEngineStateWrap,
        IBackgroundTaskService backgroundTaskService,
        LuthetusTextEditorOptions textEditorOptions,
        ITextEditorRegistryWrap textEditorRegistryWrap,
        IStorageService storageService,
        IJSRuntime jsRuntime,
        StorageSync storageSync,
        IDispatcher dispatcher)
    {
        ModelStateWrap = modelStateWrap;
        ViewModelStateWrap = viewModelStateWrap;
        GroupStateWrap = groupStateWrap;
        DiffStateWrap = diffStateWrap;
        ThemeStateWrap = themeStateWrap;
        OptionsStateWrap = optionsStateWrap;
        SearchEngineStateWrap = searchEngineStateWrap;

        _backgroundTaskService = backgroundTaskService;
        _textEditorOptions = textEditorOptions;
        _textEditorRegistryWrap = textEditorRegistryWrap;
        _storageService = storageService;
        _jsRuntime = jsRuntime;
        _storageSync = storageSync;
        _dispatcher = dispatcher;

        ModelApi = new TextEditorModelApi(this, _textEditorRegistryWrap.DecorationMapperRegistry, _textEditorRegistryWrap.CompilerServiceRegistry, _backgroundTaskService, _dispatcher);
        ViewModelApi = new TextEditorViewModelApi(this, _backgroundTaskService, ViewModelStateWrap, ModelStateWrap, _jsRuntime, _dispatcher);
        GroupApi = new TextEditorGroupApi(this, _dispatcher);
        DiffApi = new TextEditorDiffApi(this, _dispatcher);
        OptionsApi = new TextEditorOptionsApi(this, _textEditorOptions, _storageService, _storageSync, _dispatcher);
        SearchEngineApi = new TextEditorSearchEngineApi(this, _dispatcher);
    }

    public IState<TextEditorModelState> ModelStateWrap { get; }
    public IState<TextEditorViewModelState> ViewModelStateWrap { get; }
    public IState<TextEditorGroupState> GroupStateWrap { get; }
    public IState<TextEditorDiffState> DiffStateWrap { get; }
    public IState<ThemeState> ThemeStateWrap { get; }
    public IState<TextEditorOptionsState> OptionsStateWrap { get; }
    public IState<TextEditorSearchEngineState> SearchEngineStateWrap { get; }

    
#if DEBUG
    public string StorageKey => "luth_te_text-editor-options-debug";
#else
    public string StorageKey => "luth_te_text-editor-options";
#endif

    public string ThemeCssClassString => ThemeStateWrap.Value.ThemeBag.FirstOrDefault(
        x => x.Key == OptionsStateWrap.Value.Options.CommonOptions.ThemeKey)
        ?.CssClassString
            ?? ThemeFacts.VisualStudioDarkThemeClone.CssClassString;

    public ITextEditorModelApi ModelApi { get; }
    public ITextEditorViewModelApi ViewModelApi { get; }
    public ITextEditorGroupApi GroupApi { get; }
    public ITextEditorDiffApi DiffApi { get; }
    public ITextEditorOptionsApi OptionsApi { get; }
    public ITextEditorSearchEngineApi SearchEngineApi { get; }

    public void EnqueueModification(
        string modificationName,
        TextEditorCommandArgs commandArgs,
        TextEditorCommand.ModificationTask modificationTask)
    {
        _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            modificationName,
            async () =>
            {
                var model = commandArgs.TextEditorService.ModelApi.GetOrDefault(commandArgs.ModelResourceUri);
                var viewModel = commandArgs.TextEditorService.ViewModelApi.GetOrDefault(commandArgs.ViewModelKey);

                if (viewModel is null || model is null)
                    return;

                var refreshCursorsRequest = new RefreshCursorsRequest(
                    commandArgs.ViewModelKey,
                    new List<TextEditorCursorModifier>());

                refreshCursorsRequest.CursorBag.AddRange(viewModel.CursorBag
                    .Select(x => new TextEditorCursorModifier(x)));

                var primaryCursor = refreshCursorsRequest.CursorBag.FirstOrDefault(x => x.IsPrimaryCursor);

                if (primaryCursor is null)
                    return;

                await modificationTask.Invoke(
                    commandArgs,
                    model,
                    viewModel,
                    refreshCursorsRequest,
                    primaryCursor);

                if (refreshCursorsRequest is not null)
                {
                    _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                        refreshCursorsRequest.ViewModelKey,
                        inState => inState with
                        {
                            CursorBag = refreshCursorsRequest.CursorBag
                                .Select(x => x.ToCursor())
                                .ToImmutableArray()
                        }));
                }
            });
    }
}