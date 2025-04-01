using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.TextEditor.RazorLib.Installations.Displays;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

public class TextEditorInitializationBackgroundTaskGroup : IBackgroundTaskGroup
{
    public TextEditorInitializationBackgroundTaskGroup(
        IBackgroundTaskService backgroundTaskService,
        IKeymapService keymapService,
        LuthetusTextEditorConfig textEditorConfig,
        IThemeService themeService,
        ITextEditorService textEditorService,
        IEnvironmentProvider environmentProvider,
        IContextService contextService)
    {
        _backgroundTaskService = backgroundTaskService;
        _keymapService = keymapService;
        _textEditorConfig = textEditorConfig;
        _themeService = themeService;
        _textEditorService = textEditorService;
        _environmentProvider = environmentProvider;
        _contextService = contextService;
    }

    public Key<IBackgroundTask> BackgroundTaskKey { get; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public string Name { get; } = nameof(TextEditorInitializationBackgroundTaskGroup);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<TextEditorInitializationBackgroundTaskGroupWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IKeymapService _keymapService;
    private readonly LuthetusTextEditorConfig _textEditorConfig;
    private readonly IThemeService _themeService;
    private readonly ITextEditorService _textEditorService;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IContextService _contextService;

    public void Enqueue_LuthetusTextEditorInitializerOnInit()
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(TextEditorInitializationBackgroundTaskGroupWorkKind.LuthetusTextEditorInitializerOnInit);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }

    public async ValueTask Do_LuthetusTextEditorInitializerOnInit()
    {
        if (_textEditorConfig.CustomThemeRecordList is not null)
        {
            foreach (var themeRecord in _textEditorConfig.CustomThemeRecordList)
            {
                _themeService.ReduceRegisterAction(themeRecord);
            }
        }

        var initialThemeRecord = _themeService.GetThemeState().ThemeList.FirstOrDefault(
            x => x.Key == _textEditorConfig.InitialThemeKey);

        if (initialThemeRecord is not null)
            _textEditorService.OptionsApi.SetTheme(initialThemeRecord, updateStorage: false);

        await _textEditorService.OptionsApi.SetFromLocalStorageAsync().ConfigureAwait(false);

        _contextService.RegisterContextSwitchGroup(
            new ContextSwitchGroup(
                LuthetusTextEditorInitializer.ContextSwitchGroupKey,
                "Text Editor",
                () =>
                {
                    var menuOptionList = new List<MenuOptionRecord>();

                    var mainGroup = _textEditorService.GroupApi.GetGroups()
                        .FirstOrDefault(x => x.Category.Value == "main");

                    if (mainGroup is not null)
                    {
                        var viewModelList = new List<TextEditorViewModel>();

                        foreach (var viewModelKey in mainGroup.ViewModelKeyList)
                        {
                            var viewModel = _textEditorService.ViewModelApi.GetOrDefault(viewModelKey);

                            if (viewModel is not null)
                            {
                                viewModelList.Add(viewModel);

                                var absolutePath = _environmentProvider.AbsolutePathFactory(
                                    viewModel.ResourceUri.Value,
                                    false);

                                menuOptionList.Add(new MenuOptionRecord(
                                    absolutePath.NameWithExtension,
                                    MenuOptionKind.Other,
                                    onClickFunc: () =>
                                    {
                                    	_textEditorService.WorkerArbitrary.PostUnique(nameof(TextEditorInitializationBackgroundTaskGroup), async editContext =>
                                    	{
                                    		await _textEditorService.OpenInEditorAsync(
                                    			editContext,
	                                            absolutePath.Value,
	                                            true,
	                                            cursorPositionIndex: null,
	                                            new Category("main"),
	                                            viewModel.ViewModelKey);
                                    	});
                                    	return Task.CompletedTask;
                                    }));
                            }
                        }
                    }

                    var menu = menuOptionList.Count == 0
                        ? new MenuRecord(MenuRecord.NoMenuOptionsExistList)
                        : new MenuRecord(menuOptionList);

                    return Task.FromResult(menu);
                }));

        _keymapService.RegisterKeymapLayer(TextEditorKeymapDefaultFacts.HasSelectionLayer);
    }

    public IBackgroundTask? EarlyBatchOrDefault(IBackgroundTask oldEvent)
    {
        return null;
    }

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        TextEditorInitializationBackgroundTaskGroupWorkKind workKind;

        lock (_workLock)
        {
            if (!_workKindQueue.TryDequeue(out workKind))
                return ValueTask.CompletedTask;
        }

        switch (workKind)
        {
            case TextEditorInitializationBackgroundTaskGroupWorkKind.LuthetusTextEditorInitializerOnInit:
            {
                return Do_LuthetusTextEditorInitializerOnInit();
            }
            default:
            {
                Console.WriteLine($"{nameof(TextEditorInitializationBackgroundTaskGroup)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
            }
        }
    }
}
