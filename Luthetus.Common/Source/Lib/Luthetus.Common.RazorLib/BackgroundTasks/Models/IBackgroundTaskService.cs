using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public interface IBackgroundTaskService
{
    public event Action? ExecutingBackgroundTaskChanged;

    public void Enqueue(
        Key<BackgroundTask> taskKey,
        Key<BackgroundTaskQueue> queueKey,
        string name,
        Func<Task> runFunc);

    public void Enqueue(IBackgroundTask backgroundTask);
    public void RegisterQueue(BackgroundTaskQueue queue);

    public Task<IBackgroundTask?> DequeueAsync(
        Key<BackgroundTaskQueue> queueKey,
        CancellationToken cancellationToken);

    public void SetExecutingBackgroundTask(
        Key<BackgroundTaskQueue> queueKey,
        IBackgroundTask? backgroundTask);
}
