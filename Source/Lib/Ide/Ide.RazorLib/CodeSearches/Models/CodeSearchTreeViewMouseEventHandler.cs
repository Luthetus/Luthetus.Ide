using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Installations.Models;

namespace Luthetus.Ide.RazorLib.CodeSearches.Models;

public class CodeSearchTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
	private readonly ITextEditorService _textEditorService;
	private readonly LuthetusTextEditorConfig _textEditorConfig;
	private readonly IServiceProvider _serviceProvider;

	public CodeSearchTreeViewMouseEventHandler(
			ITextEditorService textEditorService,
			LuthetusTextEditorConfig textEditorConfig,
			IServiceProvider serviceProvider,
			ITreeViewService treeViewService,
			IBackgroundTaskService backgroundTaskService)
		: base(treeViewService, backgroundTaskService)
	{
		_textEditorService = textEditorService;
		_textEditorConfig = textEditorConfig;
		_serviceProvider = serviceProvider;
	}

	public override Task OnDoubleClickAsync(TreeViewCommandArgs commandArgs)
	{
		base.OnDoubleClickAsync(commandArgs);

		if (commandArgs.NodeThatReceivedMouseEvent is not TreeViewCodeSearchTextSpan treeViewCodeSearchTextSpan)
			return Task.CompletedTask;

		TreeViewCodeSearchTextSpanHelper.OpenInEditorOnClick(
			treeViewCodeSearchTextSpan,
			true,
			_textEditorService,
			_textEditorConfig,
			_serviceProvider);

		return Task.CompletedTask;
	}
}
