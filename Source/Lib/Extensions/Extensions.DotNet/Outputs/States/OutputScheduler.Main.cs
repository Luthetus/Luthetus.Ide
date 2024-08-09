using Fluxor;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;

namespace Luthetus.Extensions.DotNet.Outputs.States;

/// <inheritdoc cref="IStateScheduler"/>
public partial class OutputScheduler : IStateScheduler
{
	private readonly DotNetBackgroundTaskApi _dotNetBackgroundTaskApi;
	private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly DotNetCliOutputParser _dotNetCliOutputParser;
	private readonly ITreeViewService _treeViewService;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly IDispatcher _dispatcher;
		
	public OutputScheduler(
		DotNetBackgroundTaskApi dotNetBackgroundTaskApi,
		IBackgroundTaskService backgroundTaskService,
		DotNetCliOutputParser dotNetCliOutputParser,
		ITreeViewService treeViewService,
		IEnvironmentProvider environmentProvider,
		IDispatcher dispatcher)
	{
		_dotNetBackgroundTaskApi = dotNetBackgroundTaskApi;
		_backgroundTaskService = backgroundTaskService;
		_dotNetCliOutputParser = dotNetCliOutputParser;
		_treeViewService = treeViewService;
		_environmentProvider = environmentProvider;
		_dispatcher = dispatcher;
	}
}
