using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

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
    
    public void Enqueue_DiscoverTests()
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "DiscoverTests",
            Task_DiscoverTests);
    }
}
