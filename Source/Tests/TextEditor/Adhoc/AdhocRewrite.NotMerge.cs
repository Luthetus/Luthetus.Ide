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
	[Fact]
	public async Task Insertion_DeletionWithDeleteKindDelete()
	{
		Initialize_AdhocRewriteTest(
			string.Empty,
			out var resourceUri,
			out var cursor,
			out var textEditorService,
			out var backgroundTaskService,
			out var backgroundTaskWorker,
			out var serviceProvider);

		var content = new StringBuilder("abc123");

		await textEditorService.Post(new TextEditorWorkInsertion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
			content,
			Key<TextEditorViewModel>.Empty));

		await textEditorService.Post(new TextEditorWorkDeletion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
			"abc123".Length,
			TextEditorModelModifier.DeleteKind.Delete));

		var queue = backgroundTaskService.GetQueue(ContinuousBackgroundTaskWorker.GetQueueKey());
		Assert.Equal(1, queue.CountOfBackgroundTasks);

		var backgroundTask = (TextEditorBackgroundTask)queue.BackgroundTasks.Single();
		Assert.Equal(2, backgroundTask._workList.Count);

		Assert.IsType<TextEditorWorkInsertion>(backgroundTask._workList[0]);
		Assert.IsType<TextEditorWorkDeletion>(backgroundTask._workList[1]);

		await ConsumeQueue_AdhocRewriteTest(backgroundTaskWorker);

		var outModel = textEditorService.ModelStateWrap.Value.ModelList.First(
			x => x.ResourceUri == resourceUri);

		var text = outModel.AllText;

		Assert.Equal("abc123", text);
	}

	[Fact]
	public async Task Insertion_DeletionWithDeleteKindBackspace()
	{
		Initialize_AdhocRewriteTest(
			string.Empty,
			out var resourceUri,
			out var cursor,
			out var textEditorService,
			out var backgroundTaskService,
			out var backgroundTaskWorker,
			out var serviceProvider);

		var content = new StringBuilder("abc123");

		await textEditorService.Post(new TextEditorWorkInsertion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
			content,
			Key<TextEditorViewModel>.Empty));

		await textEditorService.Post(new TextEditorWorkDeletion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
			"abc123".Length,
			TextEditorModelModifier.DeleteKind.Backspace));

		var queue = backgroundTaskService.GetQueue(ContinuousBackgroundTaskWorker.GetQueueKey());
		Assert.Equal(1, queue.CountOfBackgroundTasks);

		var backgroundTask = (TextEditorBackgroundTask)queue.BackgroundTasks.Single();
		Assert.Equal(2, backgroundTask._workList.Count);

		Assert.IsType<TextEditorWorkInsertion>(backgroundTask._workList[0]);
		Assert.IsType<TextEditorWorkDeletion>(backgroundTask._workList[1]);

		await ConsumeQueue_AdhocRewriteTest(backgroundTaskWorker);

		var outModel = textEditorService.ModelStateWrap.Value.ModelList.First(
			x => x.ResourceUri == resourceUri);

		var text = outModel.AllText;

		Assert.Equal(string.Empty, text);
	}

	[Fact]
	public async Task DeletionWithDeleteKindDelete_Insertion()
	{
		Initialize_AdhocRewriteTest(
			string.Empty,
			out var resourceUri,
			out var cursor,
			out var textEditorService,
			out var backgroundTaskService,
			out var backgroundTaskWorker,
			out var serviceProvider);

		var content = new StringBuilder("abc123");

		await textEditorService.Post(new TextEditorWorkDeletion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
			"abc123".Length,
			TextEditorModelModifier.DeleteKind.Delete));

		await textEditorService.Post(new TextEditorWorkInsertion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
			content,
			Key<TextEditorViewModel>.Empty));

		var queue = backgroundTaskService.GetQueue(ContinuousBackgroundTaskWorker.GetQueueKey());
		Assert.Equal(1, queue.CountOfBackgroundTasks);

		var backgroundTask = (TextEditorBackgroundTask)queue.BackgroundTasks.Single();
		Assert.Equal(2, backgroundTask._workList.Count);

		Assert.IsType<TextEditorWorkDeletion>(backgroundTask._workList[0]);
		Assert.IsType<TextEditorWorkInsertion>(backgroundTask._workList[1]);

		await ConsumeQueue_AdhocRewriteTest(backgroundTaskWorker);

		var outModel = textEditorService.ModelStateWrap.Value.ModelList.First(
			x => x.ResourceUri == resourceUri);

		var text = outModel.AllText;

		Assert.Equal("abc123", text);
	}

	[Fact]
	public async Task DeletionWithDeleteKindBackspace_Insertion()
	{
		Initialize_AdhocRewriteTest(
			string.Empty,
			out var resourceUri,
			out var cursor,
			out var textEditorService,
			out var backgroundTaskService,
			out var backgroundTaskWorker,
			out var serviceProvider);

		var content = new StringBuilder("abc123");

		await textEditorService.Post(new TextEditorWorkDeletion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
			"abc123".Length,
			TextEditorModelModifier.DeleteKind.Backspace));

		await textEditorService.Post(new TextEditorWorkInsertion(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
			content,
			Key<TextEditorViewModel>.Empty));

		var queue = backgroundTaskService.GetQueue(ContinuousBackgroundTaskWorker.GetQueueKey());
		Assert.Equal(1, queue.CountOfBackgroundTasks);

		var backgroundTask = (TextEditorBackgroundTask)queue.BackgroundTasks.Single();
		Assert.Equal(2, backgroundTask._workList.Count);

		Assert.IsType<TextEditorWorkDeletion>(backgroundTask._workList[0]);
		Assert.IsType<TextEditorWorkInsertion>(backgroundTask._workList[1]);

		await ConsumeQueue_AdhocRewriteTest(backgroundTaskWorker);

		var outModel = textEditorService.ModelStateWrap.Value.ModelList.First(
			x => x.ResourceUri == resourceUri);

		var text = outModel.AllText;

		Assert.Equal("abc123", text);
	}
}