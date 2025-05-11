using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Extensions.DotNet.CompilerServices.Models;

public class CompilerServiceExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;

	public CompilerServiceExplorerTreeViewMouseEventHandler(
			IdeBackgroundTaskApi ideBackgroundTaskApi,
			ITreeViewService treeViewService,
			BackgroundTaskService backgroundTaskService)
		: base(treeViewService, backgroundTaskService)
	{
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
	}

	public override Task OnDoubleClickAsync(TreeViewCommandArgs commandArgs)
	{
		return base.OnDoubleClickAsync(commandArgs);
	}
}