namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskServiceSynchronousTests
{
    [Fact]
    public void ExecutingBackgroundTask()
    {
        /*
        public IBackgroundTask? ExecutingBackgroundTask => throw new NotImplementedException();
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void PendingBackgroundTasks()
    {
        /*
        public ImmutableArray<IBackgroundTask> PendingBackgroundTasks => throw new NotImplementedException();
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void CompletedBackgroundTasks()
    {
        /*
        public ImmutableArray<IBackgroundTask> CompletedBackgroundTasks => throw new NotImplementedException();
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ExecutingBackgroundTaskChanged()
    {
        /*
        public event Action? ExecutingBackgroundTaskChanged;
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void EnqueueA()
    {
        /*
        public void Enqueue(IBackgroundTask backgroundTask)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void EnqueueB()
    {
        /*
        public void Enqueue(
            Key<BackgroundTask> taskKey, Key<BackgroundTaskQueue> queueKey, string name, Func<Task> runFunc)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void DequeueAsync()
    {
        /*
        public Task<IBackgroundTask?> DequeueAsync(
            Key<BackgroundTaskQueue> queueKey, CancellationToken cancellationToken)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void SetExecutingBackgroundTask()
    {
        /*
        public void SetExecutingBackgroundTask(
            Key<BackgroundTaskQueue> queueKey, IBackgroundTask? backgroundTask)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void RegisterQueue()
    {
        /*
        public void RegisterQueue(BackgroundTaskQueue queue)
         */

        throw new NotImplementedException();
    }
}