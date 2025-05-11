using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Installations.Models;

namespace Luthetus.Extensions.DotNet.Outputs.Models;

public class OutputTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
	private readonly TextEditorService _textEditorService;
	private readonly LuthetusTextEditorConfig _textEditorConfig;
	private readonly IServiceProvider _serviceProvider;

	public OutputTreeViewKeyboardEventHandler(
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

	public override Task OnKeyDownAsync(TreeViewCommandArgs commandArgs)
	{
		if (commandArgs.KeyboardEventArgs is null)
			return Task.CompletedTask;

		base.OnKeyDownAsync(commandArgs);

		switch (commandArgs.KeyboardEventArgs.Code)
		{
			case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
				return InvokeOpenInEditor(commandArgs, true);
			case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
				return InvokeOpenInEditor(commandArgs, false);
		}
		
		return Task.CompletedTask;
	}

	private Task InvokeOpenInEditor(TreeViewCommandArgs commandArgs, bool shouldSetFocusToEditor)
	{
		var activeNode = commandArgs.TreeViewContainer.ActiveNode;

		if (activeNode is not TreeViewDiagnosticLine treeViewDiagnosticLine)
			return Task.CompletedTask;
			
		return OutputTextSpanHelper.OpenInEditorOnClick(
			treeViewDiagnosticLine,
			shouldSetFocusToEditor,
			_textEditorService);
	}
}
