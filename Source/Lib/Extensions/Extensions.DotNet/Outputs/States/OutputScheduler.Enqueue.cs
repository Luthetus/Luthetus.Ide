namespace Luthetus.Extensions.DotNet.Outputs.States;

public partial class OutputScheduler
{
	public void Enqueue_ConstructTreeView()
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Refresh Output",
            Task_ConstructTreeView);
    }
}
