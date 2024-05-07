using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.States;

public partial class TestExplorerSync
{
    public Task DotNetSolutionStateWrap_StateChanged()
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Refresh TestExplorer",
            async () => await DotNetSolutionStateWrap_StateChangedAsync());
    }
}