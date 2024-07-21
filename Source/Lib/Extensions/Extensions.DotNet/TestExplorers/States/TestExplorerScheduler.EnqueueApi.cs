using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.States;

public partial class TestExplorerScheduler
{
	public void Enqueue_ConstructTreeView()
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Refresh TestExplorer",
            Task_ConstructTreeView);
    }
}
