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

public partial class AdhocRewrite
{
	private async Task ConsumeQueue_AdhocRewriteTest(ContinuousBackgroundTaskWorker backgroundTaskWorker)
	{
		var cts = new CancellationTokenSource();
        var token = cts.Token;

        var consumerThread = new Thread(async () =>
		{
			await backgroundTaskWorker.StartAsync(token);
		});

		consumerThread.Start();

		await Task.Yield();
		await Task.Yield();

        await backgroundTaskWorker.StopAsync(token);
        consumerThread.Join();
        cts.Cancel();
	}

	private void Initialize_AdhocRewriteTest(
		string initialContent,
		out ResourceUri resourceUri,
		out TextEditorCursor cursor,
		out ITextEditorService textEditorService,
		out IBackgroundTaskService backgroundTaskService,
		out ContinuousBackgroundTaskWorker backgroundTaskWorker,
		out IServiceProvider serviceProvider)
	{
		var hackyBackgroundTaskService = new BackgroundTaskService();
		backgroundTaskService = hackyBackgroundTaskService;

		var queueKey = ContinuousBackgroundTaskWorker.GetQueueKey();

		var queue = new BackgroundTaskQueue(
            queueKey,
            ContinuousBackgroundTaskWorker.QUEUE_DISPLAY_NAME);

        backgroundTaskService.RegisterQueue(queue);

		var services = new ServiceCollection()
			.AddScoped<ContinuousBackgroundTaskWorker>(sp => new ContinuousBackgroundTaskWorker(
				sp.GetRequiredService<IBackgroundTaskService>(),
				sp.GetRequiredService<ILoggerFactory>()))
			.AddScoped<ILoggerFactory, NullLoggerFactory>()
			.AddScoped<IBackgroundTaskService>(_ => hackyBackgroundTaskService)
			.AddScoped<ITextEditorService, TestTextEditorService>()
			.AddFluxor(options => options.ScanAssemblies(
				typeof(LuthetusCommonConfig).Assembly,
				typeof(LuthetusTextEditorConfig).Assembly));

		serviceProvider = services.BuildServiceProvider();

		var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

		var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

		textEditorService = serviceProvider.GetRequiredService<ITextEditorService>();
		backgroundTaskWorker = serviceProvider.GetRequiredService<ContinuousBackgroundTaskWorker>();

		resourceUri = new ResourceUri("/unitTesting.txt");
		cursor = new TextEditorCursor(true);

		var inModel = new TextEditorModel(
	        resourceUri,
	        DateTime.UtcNow,
	        "txt",
	        initialContent,
	        null,
	        null,
			4_096);

		dispatcher.Dispatch(new TextEditorModelState.RegisterAction(
            TextEditorService.AuthenticatedActionKey,
            inModel));
	}
}