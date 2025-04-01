using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.FolderExplorers.Models;

public class FolderExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly ITextEditorService _textEditorService;

    public FolderExplorerTreeViewMouseEventHandler(
            IdeBackgroundTaskApi ideBackgroundTaskApi,
            ITextEditorService textEditorService,
            ITreeViewService treeViewService,
		    IBackgroundTaskService backgroundTaskService)
        : base(treeViewService, backgroundTaskService)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _textEditorService = textEditorService;
    }

    public override async Task OnDoubleClickAsync(TreeViewCommandArgs commandArgs)
    {
        await base.OnDoubleClickAsync(commandArgs).ConfigureAwait(false);

        if (commandArgs.NodeThatReceivedMouseEvent is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

		_textEditorService.WorkerArbitrary.PostUnique(nameof(FolderExplorerTreeViewMouseEventHandler), async editContext =>
		{
			await _textEditorService.OpenInEditorAsync(
				editContext,
				treeViewAbsolutePath.Item.Value,
				true,
				cursorPositionIndex: null,
				new Category("main"),
				Key<TextEditorViewModel>.NewKey());
		});
    }
}