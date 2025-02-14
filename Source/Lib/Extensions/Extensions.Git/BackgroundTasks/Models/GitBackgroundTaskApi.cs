using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Extensions.Git.ComponentRenderers.Models;
using Luthetus.Extensions.Git.Models;

namespace Luthetus.Extensions.Git.BackgroundTasks.Models;

public class GitBackgroundTaskApi
{
	private readonly LuthetusCommonApi _commonApi;
	private readonly ITerminalService _terminalService;
	private readonly GitTreeViews _gitTreeViews;
	private readonly IIdeComponentRenderers _ideComponentRenderers;
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	
	public GitBackgroundTaskApi(
		LuthetusCommonApi commonApi,
		GitTreeViews gitTreeViews,
		IIdeComponentRenderers ideComponentRenderers,
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		ITerminalService terminalService)
	{
		_commonApi = commonApi;
		_gitTreeViews = gitTreeViews;
		_ideComponentRenderers = ideComponentRenderers;
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
		_terminalService = terminalService;

		GitCliOutputParser = new GitCliOutputParser(
			this,
            _commonApi.EnvironmentProviderApi);

		Git = new GitIdeApi(
            _commonApi,
            _gitTreeViews,
			_ideComponentRenderers,
			this,
			_ideBackgroundTaskApi,
			_terminalService,
			GitCliOutputParser);
	}
	
	public GitIdeApi Git { get; }
	public GitCliOutputParser GitCliOutputParser { get; }
}
