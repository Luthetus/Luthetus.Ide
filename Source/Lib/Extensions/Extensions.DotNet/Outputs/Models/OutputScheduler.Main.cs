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
	private readonly LuthetusCommonApi _commonApi;
	private readonly DotNetBackgroundTaskApi _dotNetBackgroundTaskApi;
	private readonly DotNetCliOutputParser _dotNetCliOutputParser;
	private readonly IOutputService _outputService;
		
	public OutputScheduler(
		LuthetusCommonApi commonApi,
		DotNetBackgroundTaskApi dotNetBackgroundTaskApi,
		DotNetCliOutputParser dotNetCliOutputParser,
		IOutputService outputService)
	{
		_commonApi = commonApi;
		_dotNetBackgroundTaskApi = dotNetBackgroundTaskApi;
		_dotNetCliOutputParser = dotNetCliOutputParser;
		_outputService = outputService;
	}
}
