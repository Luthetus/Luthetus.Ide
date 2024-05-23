using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.Tests.Adhoc.Rewrite;

namespace Luthetus.TextEditor.Tests.Adhoc;

public class AdhocRewrite
{
	/// <summary>
	/// Rewriting logic related to the text editor background tasks.
	///
	/// Durations are wrong:
	/// https://github.com/xunit/visualstudio.xunit/issues/401
    /// </summary>
	[Fact]
	public async Task Aaa()
	{
		var backgroundTaskService = new BackgroundTaskService();

		var queueKey = ContinuousBackgroundTaskWorker.GetQueueKey();

		var queue = new BackgroundTaskQueue(
            queueKey,
            ContinuousBackgroundTaskWorker.QUEUE_DISPLAY_NAME);

        backgroundTaskService.RegisterQueue(queue);

		var textEditorService = new TestTextEditorService(backgroundTaskService);

		var services = new ServiceCollection()
			.AddScoped<ContinuousBackgroundTaskWorker>(sp => new ContinuousBackgroundTaskWorker(
				sp.GetRequiredService<IBackgroundTaskService>(),
				sp.GetRequiredService<ILoggerFactory>()))
			.AddScoped<ILoggerFactory, NullLoggerFactory>()
			.AddScoped<IBackgroundTaskService>(_ => backgroundTaskService)
			.AddScoped<ITextEditorService>(_ => textEditorService)
			.AddFluxor(options => options.ScanAssemblies(
				typeof(LuthetusCommonConfig).Assembly,
				typeof(LuthetusTextEditorConfig).Assembly));

		var serviceProvider = services.BuildServiceProvider();

		var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

		var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

		var resourceUri = new ResourceUri("/unitTesting.txt");
		var cursor = new TextEditorCursor(true);
		var content = new StringBuilder("abc123");

		var inModel = new TextEditorModel(
	        resourceUri,
	        DateTime.UtcNow,
	        "txt",
	        string.Empty,
	        null,
	        null,
			4_096);

		dispatcher.Dispatch(new TextEditorModelState.RegisterAction(
            TextEditorService.AuthenticatedActionKey,
            inModel));
		
		await textEditorService.Post(new TextEditorWorkInsertion(
			resourceUri,
			cursor.Key,
			cursorKey => cursor,
			content));

		var cts = new CancellationTokenSource();
        var token = cts.Token;

		var backgroundTaskWorker = serviceProvider
			.GetRequiredService<ContinuousBackgroundTaskWorker>();

		Console.WriteLine("Hello???");

        var consumerThread = new Thread(async () =>
		{
			Console.WriteLine("went-inside-consumerThread");
			await backgroundTaskWorker.StartAsync(token);
			Console.WriteLine("going-outside-consumerThread");
		});

		consumerThread.Start();

		await Task.Yield();
		await Task.Yield();

        await backgroundTaskWorker.StopAsync(token);
        consumerThread.Join();
        cts.Cancel();

		var textEditorModelStateWrap = serviceProvider
			.GetRequiredService<IState<TextEditorModelState>>();

		var outModel = textEditorModelStateWrap.Value.ModelList.First(
			x => x.ResourceUri == resourceUri);

		var text = string.IsNullOrWhiteSpace(outModel.AllText)
			? "outModel.AllText IsNullOrWhiteSpace"
			: outModel.AllText;

		Console.WriteLine(text);

		Console.WriteLine("Goodbye???");
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
