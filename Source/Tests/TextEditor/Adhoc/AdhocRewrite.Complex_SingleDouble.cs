using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Web;
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
using Luthetus.TextEditor.RazorLib.Events.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.Tests.Adhoc.Rewrite;

namespace Luthetus.TextEditor.Tests.Adhoc;

public partial class AdhocRewrite
{
	[Fact]
	public async Task Single_TextEditorWorkKeyDown()
	{
		Initialize_AdhocRewriteTest(
			string.Empty,
			out var resourceUri,
			out var cursor,
			out var textEditorService,
			out var backgroundTaskService,
			out var backgroundTaskWorker,
			out var serviceProvider);

		var optionsState = serviceProvider.GetRequiredService<IState<TextEditorOptionsState>>();
		var options = optionsState.Value.Options;

		var keyboardEventArgs = new KeyboardEventArgs { Key = "a", Code = "KeyA" };

		await textEditorService.Post(new TextEditorWorkKeyDown(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
			keyboardEventArgs,
			options,
			Key<TextEditorViewModel>.Empty));

		var queue = backgroundTaskService.GetQueue(ContinuousBackgroundTaskWorker.GetQueueKey());
		Assert.Equal(1, queue.CountOfBackgroundTasks);

		var backgroundTask = (TextEditorBackgroundTask)queue.BackgroundTasks.Single();
		Assert.Equal(1, backgroundTask._workList.Count);

		await ConsumeQueue_AdhocRewriteTest(backgroundTaskWorker);

		var outModel = textEditorService.ModelStateWrap.Value.ModelList.First(
			x => x.ResourceUri == resourceUri);

		var text = outModel.AllText;

		Assert.Equal("a", text);
	}

	[Fact]
	public async Task Double_TextEditorWorkKeyDown_CanBatch()
	{
		Initialize_AdhocRewriteTest(
			string.Empty,
			out var resourceUri,
			out var cursor,
			out var textEditorService,
			out var backgroundTaskService,
			out var backgroundTaskWorker,
			out var serviceProvider);

		var optionsState = serviceProvider.GetRequiredService<IState<TextEditorOptionsState>>();
		var options = optionsState.Value.Options;

		await textEditorService.Post(new TextEditorWorkKeyDown(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
			new KeyboardEventArgs { Key = "a", Code = "KeyA" },
			options,
			Key<TextEditorViewModel>.Empty));

		await textEditorService.Post(new TextEditorWorkKeyDown(
			resourceUri,
			cursor.Key,
			(editContext, cursorKey) => cursor,
			new KeyboardEventArgs { Key = "b", Code = "KeyB" },
			options,
			Key<TextEditorViewModel>.Empty));

		var queue = backgroundTaskService.GetQueue(ContinuousBackgroundTaskWorker.GetQueueKey());
		Assert.Equal(1, queue.CountOfBackgroundTasks);

		var backgroundTask = (TextEditorBackgroundTask)queue.BackgroundTasks.Single();
		Assert.Equal(2, backgroundTask._workList.Count);

		await ConsumeQueue_AdhocRewriteTest(backgroundTaskWorker);

		var outModel = textEditorService.ModelStateWrap.Value.ModelList.First(
			x => x.ResourceUri == resourceUri);

		var text = outModel.AllText;

		Assert.Equal("ab", text);
	}
}
