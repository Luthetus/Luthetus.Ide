using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
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
			out var backgroundTaskWorker,
			out var serviceProvider);

		var content = "abc123";

		await textEditorService.Post(new TextEditorWorkInsertion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
			content,
			Key<TextEditorViewModel>.Empty));

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
			out var backgroundTaskWorker,
			out var serviceProvider);

		await textEditorService.Post(new TextEditorWorkInsertion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
			"abc",
			Key<TextEditorViewModel>.Empty));

		await textEditorService.Post(new TextEditorWorkInsertion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
			"123",
			Key<TextEditorViewModel>.Empty));

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
			out var backgroundTaskWorker,
			out var serviceProvider);

		await textEditorService.Post(new TextEditorWorkDeletion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
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
			out var backgroundTaskWorker,
			out var serviceProvider);

		await textEditorService.Post(new TextEditorWorkDeletion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
			initialContent.Length / 2,
			TextEditorModelModifier.DeleteKind.Delete));

		await textEditorService.Post(new TextEditorWorkDeletion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
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
			out var backgroundTaskWorker,
			out var serviceProvider);

		cursor = new TextEditorCursor(0, initialContent.Length, true);

		await textEditorService.Post(new TextEditorWorkDeletion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
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
			out var backgroundTaskWorker,
			out var serviceProvider);

		cursor = new TextEditorCursor(0, initialContent.Length, true);

		await textEditorService.Post(new TextEditorWorkDeletion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
			initialContent.Length / 2,
			TextEditorModelModifier.DeleteKind.Backspace));

		await textEditorService.Post(new TextEditorWorkDeletion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
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
}
