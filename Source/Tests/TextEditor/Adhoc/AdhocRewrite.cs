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
	public async Task Single_TextEditorWorkInsertion()
	{
		Initialize_AdhocRewriteTest(
			string.Empty,
			out var resourceUri,
			out var cursor,
			out var textEditorService,
			out var backgroundTaskService,
			out var backgroundTaskWorker);

		var content = new StringBuilder("abc123");

		await textEditorService.Post(new TextEditorWorkInsertion(
			resourceUri,
			cursor.Key,
			cursorKey => cursor,
			content));

		var queue = backgroundTaskService.GetQueue(ContinuousBackgroundTaskWorker.GetQueueKey());
		Assert.Equal(1, queue.CountOfBackgroundTasks);

		var backgroundTask = (TextEditorBackgroundTask)queue.BackgroundTasks.Single();
		Assert.Equal(1, backgroundTask._workList.Count);

		await ConsumeQueue_AdhocRewriteTest(backgroundTaskWorker);

		var outModel = textEditorService.ModelStateWrap.Value.ModelList.First(
			x => x.ResourceUri == resourceUri);

		var text = string.IsNullOrWhiteSpace(outModel.AllText)
			? "outModel.AllText IsNullOrWhiteSpace"
			: outModel.AllText;

		Assert.Equal("abc123", text);
	}

	/// <summary>
	/// Rewriting logic related to the text editor background tasks.
	///
	/// Durations are wrong:
	/// https://github.com/xunit/visualstudio.xunit/issues/401
    /// </summary>
	[Fact]
	public async Task Double_TextEditorWorkInsertion()
	{
		Initialize_AdhocRewriteTest(
			string.Empty,
			out var resourceUri,
			out var cursor,
			out var textEditorService,
			out var backgroundTaskService,
			out var backgroundTaskWorker);

		await textEditorService.Post(new TextEditorWorkInsertion(
			resourceUri,
			cursor.Key,
			cursorKey => cursor,
			new StringBuilder("abc")));

		await textEditorService.Post(new TextEditorWorkInsertion(
			resourceUri,
			cursor.Key,
			cursorKey => cursor,
			new StringBuilder("123")));

		var queue = backgroundTaskService.GetQueue(ContinuousBackgroundTaskWorker.GetQueueKey());
		Assert.Equal(1, queue.CountOfBackgroundTasks);

		var backgroundTask = (TextEditorBackgroundTask)queue.BackgroundTasks.Single();
		Assert.Equal(1, backgroundTask._workList.Count);

		await ConsumeQueue_AdhocRewriteTest(backgroundTaskWorker);

		var outModel = textEditorService.ModelStateWrap.Value.ModelList.First(
			x => x.ResourceUri == resourceUri);

		var text = string.IsNullOrWhiteSpace(outModel.AllText)
			? "outModel.AllText IsNullOrWhiteSpace"
			: outModel.AllText;

		Assert.Equal("abc123", text);
	}

	[Fact]
	public async Task Single_TextEditorWorkDeletion_DeleteKindDelete()
	{
		var initialContent = "abc123";

		Initialize_AdhocRewriteTest(
			initialContent,
			out var resourceUri,
			out var cursor,
			out var textEditorService,
			out var backgroundTaskService,
			out var backgroundTaskWorker);

		await textEditorService.Post(new TextEditorWorkDeletion(
			resourceUri,
			cursor.Key,
			cursorKey => cursor,
			initialContent.Length,
			TextEditorModelModifier.DeleteKind.Delete));

		var queue = backgroundTaskService.GetQueue(ContinuousBackgroundTaskWorker.GetQueueKey());
		Assert.Equal(1, queue.CountOfBackgroundTasks);

		var backgroundTask = (TextEditorBackgroundTask)queue.BackgroundTasks.Single();
		Assert.Equal(1, backgroundTask._workList.Count);

		await ConsumeQueue_AdhocRewriteTest(backgroundTaskWorker);

		var outModel = textEditorService.ModelStateWrap.Value.ModelList.First(
			x => x.ResourceUri == resourceUri);

		var text = outModel.AllText;

		Assert.Equal(string.Empty, text);
	}

	[Fact]
	public async Task Double_TextEditorWorkDeletion_DeleteKindDelete()
	{
		var initialContent = "abc123";

		Initialize_AdhocRewriteTest(
			initialContent,
			out var resourceUri,
			out var cursor,
			out var textEditorService,
			out var backgroundTaskService,
			out var backgroundTaskWorker);

		await textEditorService.Post(new TextEditorWorkDeletion(
			resourceUri,
			cursor.Key,
			cursorKey => cursor,
			initialContent.Length / 2,
			TextEditorModelModifier.DeleteKind.Delete));

		await textEditorService.Post(new TextEditorWorkDeletion(
			resourceUri,
			cursor.Key,
			cursorKey => cursor,
			initialContent.Length / 2,
			TextEditorModelModifier.DeleteKind.Delete));

		var queue = backgroundTaskService.GetQueue(ContinuousBackgroundTaskWorker.GetQueueKey());
		Assert.Equal(1, queue.CountOfBackgroundTasks);

		var backgroundTask = (TextEditorBackgroundTask)queue.BackgroundTasks.Single();
		Assert.Equal(1, backgroundTask._workList.Count);

		await ConsumeQueue_AdhocRewriteTest(backgroundTaskWorker);

		var outModel = textEditorService.ModelStateWrap.Value.ModelList.First(
			x => x.ResourceUri == resourceUri);

		var text = outModel.AllText;

		Assert.Equal(string.Empty, text);
	}

	[Fact]
	public async Task Single_TextEditorWorkDeletion_DeleteKindBackspace()
	{
		var initialContent = "abc123";

		Initialize_AdhocRewriteTest(
			initialContent,
			out var resourceUri,
			out var cursor,
			out var textEditorService,
			out var backgroundTaskService,
			out var backgroundTaskWorker);

		cursor = new TextEditorCursor(0, initialContent.Length, true);

		await textEditorService.Post(new TextEditorWorkDeletion(
			resourceUri,
			cursor.Key,
			cursorKey => cursor,
			initialContent.Length,
			TextEditorModelModifier.DeleteKind.Backspace));

		var queue = backgroundTaskService.GetQueue(ContinuousBackgroundTaskWorker.GetQueueKey());
		Assert.Equal(1, queue.CountOfBackgroundTasks);

		var backgroundTask = (TextEditorBackgroundTask)queue.BackgroundTasks.Single();
		Assert.Equal(1, backgroundTask._workList.Count);

		await ConsumeQueue_AdhocRewriteTest(backgroundTaskWorker);

		var outModel = textEditorService.ModelStateWrap.Value.ModelList.First(
			x => x.ResourceUri == resourceUri);

		var text = outModel.AllText;

		Assert.Equal(string.Empty, text);
	}

	[Fact]
	public async Task Double_TextEditorWorkDeletion_DeleteKindBackspace()
	{
		var initialContent = "abc123";

		Initialize_AdhocRewriteTest(
			initialContent,
			out var resourceUri,
			out var cursor,
			out var textEditorService,
			out var backgroundTaskService,
			out var backgroundTaskWorker);

		cursor = new TextEditorCursor(0, initialContent.Length, true);

		await textEditorService.Post(new TextEditorWorkDeletion(
			resourceUri,
			cursor.Key,
			cursorKey => cursor,
			initialContent.Length / 2,
			TextEditorModelModifier.DeleteKind.Backspace));

		await textEditorService.Post(new TextEditorWorkDeletion(
			resourceUri,
			cursor.Key,
			cursorKey => cursor,
			initialContent.Length / 2,
			TextEditorModelModifier.DeleteKind.Backspace));

		var queue = backgroundTaskService.GetQueue(ContinuousBackgroundTaskWorker.GetQueueKey());
		Assert.Equal(1, queue.CountOfBackgroundTasks);

		var backgroundTask = (TextEditorBackgroundTask)queue.BackgroundTasks.Single();
		Assert.Equal(1, backgroundTask._workList.Count);

		await ConsumeQueue_AdhocRewriteTest(backgroundTaskWorker);

		var outModel = textEditorService.ModelStateWrap.Value.ModelList.First(
			x => x.ResourceUri == resourceUri);

		var text = outModel.AllText;

		Assert.Equal(string.Empty, text);
	}

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
		out ContinuousBackgroundTaskWorker backgroundTaskWorker)
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

		var serviceProvider = services.BuildServiceProvider();

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
