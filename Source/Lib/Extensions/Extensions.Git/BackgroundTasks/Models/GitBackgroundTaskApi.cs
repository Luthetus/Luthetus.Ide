using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Extensions.Git.ComponentRenderers.Models;
using Luthetus.Extensions.Git.Models;

namespace Luthetus.Extensions.Git.BackgroundTasks.Models;

public class GitBackgroundTaskApi
{
	private readonly ITerminalService _terminalService;
	private readonly GitTreeViews _gitTreeViews;
	private readonly IIdeComponentRenderers _ideComponentRenderers;
	private readonly ITreeViewService _treeViewService;
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly INotificationService _notificationService;
	private readonly IDispatcher _dispatcher;
	
	public GitBackgroundTaskApi(
		GitTreeViews gitTreeViews,
		IIdeComponentRenderers ideComponentRenderers,
		ITreeViewService treeViewService,
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		ITerminalService terminalService,
        IEnvironmentProvider environmentProvider,
        IBackgroundTaskService backgroundTaskService,
        ICommonComponentRenderers commonComponentRenderers,
        INotificationService notificationService,
        IDispatcher dispatcher)
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
        _dispatcher = dispatcher;

		GitCliOutputParser = new GitCliOutputParser(
			_dispatcher,
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
            _notificationService,
			_dispatcher);
	}
	
	public GitIdeApi Git { get; }
	public GitCliOutputParser GitCliOutputParser { get; }
}
