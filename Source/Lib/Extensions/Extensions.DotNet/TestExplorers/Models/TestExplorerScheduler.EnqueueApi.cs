using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.Models;

public partial class TestExplorerScheduler
{
	public void Enqueue_ConstructTreeView()
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.IndefiniteQueueKey,
            "Construct TreeView TestExplorer",
            Task_ConstructTreeView);
    }
    
    public void Enqueue_DiscoverTests()
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.IndefiniteQueueKey,
            "Discover Tests TestExplorer",
            Task_DiscoverTests);
    }
}
