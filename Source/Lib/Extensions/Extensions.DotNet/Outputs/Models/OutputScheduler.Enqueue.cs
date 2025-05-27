using System.Collections.Concurrent;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Extensions.DotNet.Outputs.Models;

public partial class OutputScheduler : IBackgroundTaskGroup
{
    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly ConcurrentQueue<OutputSchedulerWorkKind> _workKindQueue = new();

    public void Enqueue(OutputSchedulerWorkKind outputSchedulerWorkKind)
    {
        _workKindQueue.Enqueue(outputSchedulerWorkKind);
        _backgroundTaskService.Continuous_EnqueueGroup(this);
    }
    
    public ValueTask HandleEvent()
    {
        if (!_workKindQueue.TryDequeue(out OutputSchedulerWorkKind workKind))
            return ValueTask.CompletedTask;

        switch (workKind)
        {
            case OutputSchedulerWorkKind.ConstructTreeView:
                return Do_ConstructTreeView();
            default:
                Console.WriteLine($"{nameof(OutputScheduler)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
        }
    }
}
