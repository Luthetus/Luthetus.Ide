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
	private readonly LuthetusCommonApi _commonApi;
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly ITextEditorService _textEditorService;

	public SolutionExplorerTreeViewMouseEventHandler(
			LuthetusCommonApi commonApi,
			IdeBackgroundTaskApi ideBackgroundTaskApi,
			ITextEditorService textEditorService)
		: base(treeViewService, backgroundTaskService)
	{
		_commonApi = commonApi;
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
		_textEditorService = textEditorService;
	}

	public override Task OnDoubleClickAsync(TreeViewCommandArgs commandArgs)
	{
		base.OnDoubleClickAsync(commandArgs);

		if (commandArgs.NodeThatReceivedMouseEvent is not TreeViewNamespacePath treeViewNamespacePath)
			return Task.CompletedTask;
			
		return _textEditorService.OpenInEditorAsync(
			treeViewNamespacePath.Item.AbsolutePath.Value,
			true,
			null,
			new Category("main"),
			Key<TextEditorViewModel>.NewKey());
	}
}