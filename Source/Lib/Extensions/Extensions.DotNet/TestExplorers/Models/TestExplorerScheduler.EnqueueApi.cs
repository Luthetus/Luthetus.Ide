using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.Models;

public partial class TestExplorerScheduler : IBackgroundTaskGroup
{
    public Key<IBackgroundTask> BackgroundTaskKey { get; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public string Name { get; } = nameof(TestExplorerScheduler);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<TestExplorerSchedulerWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();

    public void Enqueue_ConstructTreeView()
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(TestExplorerSchedulerWorkKind.ConstructTreeView);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public void Enqueue_DiscoverTests()
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(TestExplorerSchedulerWorkKind.DiscoverTests);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }

    public IBackgroundTask? EarlyBatchOrDefault(IBackgroundTask oldEvent)
    {
        return null;
    }

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        TestExplorerSchedulerWorkKind workKind;

        lock (_workLock)
        {
            if (!_workKindQueue.TryDequeue(out workKind))
                return ValueTask.CompletedTask;
        }

        switch (workKind)
        {
            case TestExplorerSchedulerWorkKind.ConstructTreeView:
            {
                return Do_ConstructTreeView();
            }
            case TestExplorerSchedulerWorkKind.DiscoverTests:
            {
                return Do_DiscoverTests();
            }
            default:
            {
                return ValueTask.CompletedTask;
            }
        }
    }
}
