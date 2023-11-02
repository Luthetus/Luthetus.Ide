using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskServiceTests
{
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
        public async Task<IBackgroundTask?> DequeueAsync(
            Key<BackgroundTaskQueue> queueKey, CancellationToken cancellationToken)
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

    [Fact]
    public void SetExecutingBackgroundTask()
    {
        /*
        public void SetExecutingBackgroundTask(
            Key<BackgroundTaskQueue> queueKey, IBackgroundTask? backgroundTask)
         */

        throw new NotImplementedException();
    }
}
