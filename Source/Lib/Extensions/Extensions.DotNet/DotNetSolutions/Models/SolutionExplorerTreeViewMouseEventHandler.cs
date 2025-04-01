using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.Namespaces.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Models;

public class SolutionExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly ITextEditorService _textEditorService;

	public SolutionExplorerTreeViewMouseEventHandler(
			IdeBackgroundTaskApi ideBackgroundTaskApi,
			ITextEditorService textEditorService,
			ITreeViewService treeViewService,
			IBackgroundTaskService backgroundTaskService)
		: base(treeViewService, backgroundTaskService)
	{
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
		_textEditorService = textEditorService;
	}

	public override Task OnDoubleClickAsync(TreeViewCommandArgs commandArgs)
	{
		base.OnDoubleClickAsync(commandArgs);

		if (commandArgs.NodeThatReceivedMouseEvent is not TreeViewNamespacePath treeViewNamespacePath)
			return Task.CompletedTask;
		
		_textEditorService.WorkerArbitrary.PostUnique(nameof(SolutionExplorerTreeViewMouseEventHandler), async editContext =>
		{
			await _textEditorService.OpenInEditorAsync(
				editContext,
				treeViewNamespacePath.Item.AbsolutePath.Value,
				true,
				null,
				new Category("main"),
				Key<TextEditorViewModel>.NewKey());
		});
		return Task.CompletedTask;
	}
}