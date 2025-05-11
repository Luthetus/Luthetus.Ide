using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;

namespace Luthetus.Extensions.DotNet.Outputs.Models;

/// <inheritdoc cref="IStateScheduler"/>
public partial class OutputScheduler : IStateScheduler
{
	private readonly DotNetBackgroundTaskApi _dotNetBackgroundTaskApi;
	private readonly BackgroundTaskService _backgroundTaskService;
	private readonly DotNetCliOutputParser _dotNetCliOutputParser;
	private readonly ITreeViewService _treeViewService;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly IOutputService _outputService;
		
	public OutputScheduler(
		DotNetBackgroundTaskApi dotNetBackgroundTaskApi,
		BackgroundTaskService backgroundTaskService,
		DotNetCliOutputParser dotNetCliOutputParser,
		ITreeViewService treeViewService,
		IEnvironmentProvider environmentProvider,
		IOutputService outputService)
	{
		_dotNetBackgroundTaskApi = dotNetBackgroundTaskApi;
		_backgroundTaskService = backgroundTaskService;
		_dotNetCliOutputParser = dotNetCliOutputParser;
		_treeViewService = treeViewService;
		_environmentProvider = environmentProvider;
		_outputService = outputService;
	}
}
