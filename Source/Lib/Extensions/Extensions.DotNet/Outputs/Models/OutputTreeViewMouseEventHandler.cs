using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Installations.Models;

namespace Luthetus.Extensions.DotNet.Outputs.Models;

public class OutputTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
	private readonly TextEditorService _textEditorService;
	private readonly LuthetusTextEditorConfig _textEditorConfig;
	private readonly IServiceProvider _serviceProvider;

	public OutputTreeViewMouseEventHandler(
			TextEditorService textEditorService,
			LuthetusTextEditorConfig textEditorConfig,
			IServiceProvider serviceProvider,
			ITreeViewService treeViewService,
			BackgroundTaskService backgroundTaskService)
		: base(treeViewService, backgroundTaskService)
	{
		_textEditorService = textEditorService;
		_textEditorConfig = textEditorConfig;
		_serviceProvider = serviceProvider;
	}

	public override Task OnDoubleClickAsync(TreeViewCommandArgs commandArgs)
	{
		base.OnDoubleClickAsync(commandArgs);

		if (commandArgs.NodeThatReceivedMouseEvent is not TreeViewDiagnosticLine treeViewDiagnosticLine)
			return Task.CompletedTask;
			
		return OutputTextSpanHelper.OpenInEditorOnClick(
			treeViewDiagnosticLine,
			true,
			_textEditorService);
	}
}
