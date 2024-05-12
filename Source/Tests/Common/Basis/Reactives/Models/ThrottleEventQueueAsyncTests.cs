using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.Tests.Basis.Reactives.Models;

/// <summary>
/// <see cref="ThrottleEventQueueAsync"/>
/// </summary>
public class ThrottleEventQueueAsyncTests
{
    /// <summary>
    /// <see cref="ThrottleEventQueueAsync.EnqueueAsync(IBackgroundTask)"/>
    /// <see cref="ThrottleEventQueueAsync.DequeueOrDefaultAsync()"/>
    /// <br/>----<br/>
    /// <see cref="ThrottleEventQueueAsync.Count"/>
    /// <see cref="ThrottleEventQueueAsync.ThrottleEventList"/>
    /// </summary>
    [Fact]
    public async Task Enqueue_One()
    {
        var queue = new ThrottleEventQueueAsync();

        var cts = new CancellationTokenSource();
        var token = cts.Token;

        var thread = new Thread(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                var task = await queue.DequeueOrDefaultAsync();
                await task.HandleEvent(CancellationToken.None);
            }
        });

        int i = 0;

        var name = "Increment";
        var identifier = "Increment";
        var workItem = new Func<CancellationToken, Task>(_ =>
        {
            i++;
            return Task.CompletedTask;
        });

        await queue.EnqueueAsync(new SimpleBatchBackgroundTask(name, identifier, workItem));

        // The thread which dequeues has not yet been started
        Assert.Equal(0, i);
        Assert.Equal(1, queue.Count);

        thread.Start();
        await queue.StopFurtherEnqueuesAsync();

        await queue.UntilIsEmptyAsync();
        thread.Join();
        Assert.Equal(1, i);

        cts.Cancel();
    }

    [Fact]
    public async Task Enqueue_Two()
    {
        var queue = new ThrottleEventQueueAsync();

        var cts = new CancellationTokenSource();
        var token = cts.Token;

        var thread = new Thread(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                var task = await queue.DequeueOrDefaultAsync();
                await task.HandleEvent(CancellationToken.None);
            }
        });

        int i = 0;

        var name = "Increment";
        var identifier = "Increment";
        var workItem = new Func<CancellationToken, Task>(_ =>
        {
            i++;
            return Task.CompletedTask;
        });
        
        await queue.EnqueueAsync(new SimpleBatchBackgroundTask(name, identifier, workItem));
        await queue.EnqueueAsync(new SimpleBatchBackgroundTask(name, identifier, workItem));

        // The thread which dequeues has not yet been started
        Assert.Equal(0, i);
        Assert.Equal(1, queue.Count);

        thread.Start();
        await queue.StopFurtherEnqueuesAsync();

        await queue.UntilIsEmptyAsync();
        thread.Join();
        Assert.Equal(2, i);

        cts.Cancel();
    }

    [Fact]
    public async Task Enqueue_Three()
    {
        var queue = new ThrottleEventQueueAsync();

        var cts = new CancellationTokenSource();
        var token = cts.Token;

        var thread = new Thread(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                var task = await queue.DequeueOrDefaultAsync();
                await task.HandleEvent(CancellationToken.None);
            }
        });

        int i = 0;

        var name = "Increment";
        var identifier = "Increment";
        var workItem = new Func<CancellationToken, Task>(_ =>
        {
            i++;
            return Task.CompletedTask;
        });

        await queue.EnqueueAsync(new SimpleBatchBackgroundTask(name, identifier, workItem));
        await queue.EnqueueAsync(new SimpleBatchBackgroundTask(name, identifier, workItem));
        await queue.EnqueueAsync(new SimpleBatchBackgroundTask(name, identifier, workItem));

        // The thread which dequeues has not yet been started
        Assert.Equal(0, i);
        Assert.Equal(1, queue.Count);

        thread.Start();
        await queue.StopFurtherEnqueuesAsync();

        await queue.UntilIsEmptyAsync();
        thread.Join();
        Assert.Equal(3, i);

        cts.Cancel();
    }

    [Fact]
    public async Task Enqueue_Four()
    {
        var queue = new ThrottleEventQueueAsync();

        var cts = new CancellationTokenSource();
        var token = cts.Token;

        var thread = new Thread(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                var task = await queue.DequeueOrDefaultAsync();
                await task.HandleEvent(CancellationToken.None);
            }
        });

        int i = 0;

        var name = "Increment";
        var identifier = "Increment";
        var workItem = new Func<CancellationToken, Task>(_ =>
        {
            i++;
            return Task.CompletedTask;
        });

        await queue.EnqueueAsync(new SimpleBatchBackgroundTask(name, identifier, workItem));
        await queue.EnqueueAsync(new SimpleBatchBackgroundTask(name, identifier, workItem));
        await queue.EnqueueAsync(new SimpleBatchBackgroundTask(name, identifier, workItem));
        await queue.EnqueueAsync(new SimpleBatchBackgroundTask(name, identifier, workItem));

        // The thread which dequeues has not yet been started
        Assert.Equal(0, i);
        Assert.Equal(1, queue.Count);

        thread.Start();
        await queue.StopFurtherEnqueuesAsync();

        await queue.UntilIsEmptyAsync();
        thread.Join();
        Assert.Equal(4, i);

        cts.Cancel();
    }
}
