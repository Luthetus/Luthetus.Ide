using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Extensions.Git.Models;
using Luthetus.Extensions.Git.States;

namespace Luthetus.Extensions.Git.BackgroundTasks.Models;

public class GitBackgroundTaskApi
{
	private readonly IState<GitState> _gitStateWrap;
	private readonly IState<TerminalState> _terminalStateWrap;
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly GitCliOutputParser _gitCliOutputParser;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly INotificationService _notificationService;
	private readonly IDispatcher _dispatcher;
	
	public GitBackgroundTaskApi(
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		IState<GitState> gitStateWrap,
		IState<TerminalState> terminalStateWrap,
        GitCliOutputParser gitCliOutputParser,
        IEnvironmentProvider environmentProvider,
        IBackgroundTaskService backgroundTaskService,
        ICommonComponentRenderers commonComponentRenderers,
        INotificationService notificationService,
        IDispatcher dispatcher)
	{
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
		_gitStateWrap = gitStateWrap;
		_terminalStateWrap = terminalStateWrap;
		_gitCliOutputParser = gitCliOutputParser;
		_environmentProvider = environmentProvider;
        _backgroundTaskService = backgroundTaskService;
        _commonComponentRenderers = commonComponentRenderers;
        _notificationService = notificationService;
        _dispatcher = dispatcher;
		
		Git = new GitIdeApi(
			this,
			_ideBackgroundTaskApi,
			_terminalStateWrap,
			_gitStateWrap,
			_gitCliOutputParser,
			_environmentProvider,
			_backgroundTaskService,
            _commonComponentRenderers,
            _notificationService,
			_dispatcher);
	}
	
	public GitIdeApi Git { get; }
}
