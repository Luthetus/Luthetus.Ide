using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Extensions.Git.ComponentRenderers.Models;
using Luthetus.Extensions.Git.Models;
using Luthetus.Extensions.Git.Displays;

namespace Luthetus.Extensions.Git.BackgroundTasks.Models;

public class GitBackgroundTaskApi : IBackgroundTaskGroup
{
	private readonly ITerminalService _terminalService;
	private readonly GitTreeViews _gitTreeViews;
	private readonly IIdeComponentRenderers _ideComponentRenderers;
	private readonly ITreeViewService _treeViewService;
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly BackgroundTaskService _backgroundTaskService;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly CommonBackgroundTaskApi _commonBackgroundTaskApi;
	private readonly INotificationService _notificationService;
    private readonly IPanelService _panelService;
    private readonly IDialogService _dialogService;

    public GitBackgroundTaskApi(
		GitTreeViews gitTreeViews,
		IIdeComponentRenderers ideComponentRenderers,
		ITreeViewService treeViewService,
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		ITerminalService terminalService,
        IEnvironmentProvider environmentProvider,
        BackgroundTaskService backgroundTaskService,
        ICommonComponentRenderers commonComponentRenderers,
        CommonBackgroundTaskApi commonBackgroundTaskApi,
        INotificationService notificationService,
        IPanelService panelService,
        IDialogService dialogService)
	{
		_gitTreeViews = gitTreeViews;
		_ideComponentRenderers = ideComponentRenderers;
		_treeViewService = treeViewService;
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
		_terminalService = terminalService;
		_environmentProvider = environmentProvider;
        _backgroundTaskService = backgroundTaskService;
        _commonComponentRenderers = commonComponentRenderers;
        _notificationService = notificationService;
        _panelService = panelService;
        _dialogService = dialogService;
        _commonComponentRenderers = commonComponentRenderers;

        GitCliOutputParser = new GitCliOutputParser(
			this,
			_environmentProvider);

		Git = new GitIdeApi(
			_gitTreeViews,
			_ideComponentRenderers,
			_treeViewService,
			this,
			_ideBackgroundTaskApi,
			_terminalService,
			GitCliOutputParser,
			_environmentProvider,
			_backgroundTaskService,
            _commonComponentRenderers,
            _notificationService);
	}
	
	public GitIdeApi Git { get; }
	public GitCliOutputParser GitCliOutputParser { get; }

    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public string Name { get; } = nameof(GitIdeApi);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<GitBackgroundTaskApiWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();

    public void Enqueue_LuthetusExtensionsGitInitializerOnInit()
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(GitBackgroundTaskApiWorkKind.LuthetusExtensionsGitInitializerOnInit);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
	
	public ValueTask Do_LuthetusExtensionsGitInitializerOnInit()
    {
        InitializePanelTabs();
        return ValueTask.CompletedTask;
    }

    private void InitializePanelTabs()
    {
        InitializeLeftPanelTabs();
    }

    private void InitializeLeftPanelTabs()
    {
        var leftPanel = PanelFacts.GetTopLeftPanelGroup(_panelService.GetPanelState());
        leftPanel.PanelService = _panelService;

        // gitPanel
        var gitPanel = new Panel(
        "Git Changes",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.GitContext.ContextKey,
            typeof(GitDisplay),
            null,
            _panelService,
            _dialogService,
            _commonBackgroundTaskApi);
        _panelService.RegisterPanel(gitPanel);
        _panelService.RegisterPanelTab(leftPanel.Key, gitPanel, false);
    }

    public IBackgroundTaskGroup? EarlyBatchOrDefault(IBackgroundTaskGroup oldEvent)
	{
		return null;
	}
	
	public ValueTask HandleEvent(CancellationToken cancellationToken)
	{
        GitBackgroundTaskApiWorkKind workKind;
		
		lock (_workLock)
		{
			if (!_workKindQueue.TryDequeue(out workKind))
				return ValueTask.CompletedTask;
		}
			
		switch (workKind)
		{
			case GitBackgroundTaskApiWorkKind.LuthetusExtensionsGitInitializerOnInit:
			{
				return Do_LuthetusExtensionsGitInitializerOnInit();
			}
			default:
			{
				Console.WriteLine($"{nameof(GitBackgroundTaskApi)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
			}
		}
	}
}
