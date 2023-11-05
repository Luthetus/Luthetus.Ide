using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.Tests.Basis.BackgroundTasks.Models;

/// <summary>
/// <see cref="BackgroundTaskService"/>
/// </summary>
public class BackgroundTaskServiceTests
{
    /// <summary>
    /// <see cref="BackgroundTaskService.Enqueue(IBackgroundTask)"/>
    /// </summary>
    [Fact]
    public void EnqueueA()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="BackgroundTaskService.Enqueue(Key{BackgroundTask}, Key{BackgroundTaskQueue}, string, Func{Task})"/>
    /// </summary>
    [Fact]
    public void EnqueueB()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="BackgroundTaskService.DequeueAsync(Key{BackgroundTaskQueue}, CancellationToken)"/>
    /// </summary>
    [Fact]
    public void DequeueAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="BackgroundTaskService.RegisterQueue(BackgroundTaskQueue)"/>
    /// </summary>
    [Fact]
    public void RegisterQueue()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="BackgroundTaskService.SetExecutingBackgroundTask(Key{BackgroundTaskQueue}, IBackgroundTask?)"/>
    /// </summary>
    [Fact]
    public void SetExecutingBackgroundTask()
    {
        throw new NotImplementedException();
    }
}
