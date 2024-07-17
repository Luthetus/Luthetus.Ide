using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Namespaces.Models;

namespace Luthetus.Extensions.DotNet.CompilerServices.Models;

public class CompilerServiceExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;

	public CompilerServiceExplorerTreeViewMouseEventHandler(
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		ITreeViewService treeViewService,
		IBackgroundTaskService backgroundTaskService)
		: base(treeViewService, backgroundTaskService)
	{
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
	}

	public override Task OnDoubleClickAsync(TreeViewCommandArgs commandArgs)
	{
		base.OnDoubleClickAsync(commandArgs);

		if (commandArgs.NodeThatReceivedMouseEvent is not TreeViewNamespacePath treeViewNamespacePath)
			return Task.CompletedTask;

		_ideBackgroundTaskApi.Editor.OpenInEditor(treeViewNamespacePath.Item.AbsolutePath, true);
		return Task.CompletedTask;
	}
}