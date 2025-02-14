using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Extensions.DotNet.Outputs.Models;

public partial class OutputScheduler
{
	public void Enqueue_ConstructTreeView()
    {
        _commonApi.BackgroundTaskApi.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            "Refresh Output",
            Task_ConstructTreeView);
    }
}
