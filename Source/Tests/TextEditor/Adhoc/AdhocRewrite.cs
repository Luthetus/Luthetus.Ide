using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.Tests.Adhoc.Rewrite;

namespace Luthetus.TextEditor.Tests.Adhoc;

public class AdhocRewrite
{
	/// <summary>
	/// Rewriting logic related to the text editor background tasks.
    /// </summary>
	[Fact]
	public void Aaa()
	{
		var backgroundTaskService = new BackgroundTaskService();
	}

	/// <summary>
	/// Rewriting logic related to the text editor background tasks.
    /// </summary>
	[Fact]
	public async Task Bbb()
	{
		var queue = new ThrottleEventQueueAsync();

		var cts = new CancellationTokenSource();
        var token = cts.Token;

        var consumerThread = new Thread(async () =>
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

        consumerThread.Start();
        await queue.StopFurtherEnqueuesAsync();

        await queue.UntilIsEmptyAsync();
        consumerThread.Join();
        Assert.Equal(1, i);

        cts.Cancel();
	}
}
