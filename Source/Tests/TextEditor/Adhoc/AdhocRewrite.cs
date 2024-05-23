using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
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
    /// </summary>
	[Fact]
	public void Aaa()
	{
		var backgroundTaskService = new BackgroundTaskService();
		var textEditorService = new TestTextEditorService(backgroundTaskService);

		var services = new ServiceCollection()
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

		var model = new TextEditorModel(
	        resourceUri,
	        DateTime.UtcNow,
	        "txt",
	        string.Empty,
	        null,
	        null,
			4_096);

		dispatcher.Dispatch(new TextEditorModelState.RegisterAction(
            TextEditorService.AuthenticatedActionKey,
            model));
		
		textEditorService.Post(new TextEditorWorkInsertion(
			resourceUri,
			cursor.Key,
			cursorKey => cursor,
			content));
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
