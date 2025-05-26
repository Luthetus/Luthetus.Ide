using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Extensions.DotNet.Outputs.Models;

public partial class OutputScheduler : IBackgroundTaskGroup
{
    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();
    public string Name { get; } = nameof(OutputScheduler);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<OutputSchedulerWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();

    public void Enqueue_ConstructTreeView()
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(OutputSchedulerWorkKind.ConstructTreeView);
            _backgroundTaskService.Continuous_EnqueueGroup(this);
        }
    }
    
    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        OutputSchedulerWorkKind workKind;

        lock (_workLock)
        {
            if (!_workKindQueue.TryDequeue(out workKind))
                return ValueTask.CompletedTask;
        }

        switch (workKind)
        {
            case OutputSchedulerWorkKind.ConstructTreeView:
            {
                return Do_ConstructTreeView();
            }
            default:
            {
                Console.WriteLine($"{nameof(OutputScheduler)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
            }
        }
    }
}
