using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.Extensions.DotNet.Outputs.States;

/// <inheritdoc cref="IStateScheduler"/>
public partial class OutputScheduler : IStateScheduler
{
	private readonly DotNetBackgroundTaskApi _dotNetBackgroundTaskApi;
	private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly DotNetCliOutputParser _dotNetCliOutputParser;

	private readonly Throttle _throttleCreateTreeView = new Throttle(TimeSpan.FromMilliseconds(500));
	
	public OutputScheduler(
		DotNetBackgroundTaskApi dotNetBackgroundTaskApi,
		IBackgroundTaskService backgroundTaskService,
		DotNetCliOutputParser dotNetCliOutputParser)
	{
		_dotNetBackgroundTaskApi = dotNetBackgroundTaskApi;
		_backgroundTaskService = backgroundTaskService;
		_dotNetCliOutputParser = dotNetCliOutputParser;
	}
}
