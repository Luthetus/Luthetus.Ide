using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Extensions.DotNet.CompilerServices.Models;

public class CompilerServiceExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;

	public CompilerServiceExplorerTreeViewKeyboardEventHandler(
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		ITreeViewService treeViewService,
		BackgroundTaskService backgroundTaskService)
		: base(treeViewService, backgroundTaskService)
	{
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
	}

	public override Task OnKeyDownAsync(TreeViewCommandArgs commandArgs)
	{
		if (commandArgs.KeyboardEventArgs is null)
			return Task.CompletedTask;

		base.OnKeyDownAsync(commandArgs);

		switch (commandArgs.KeyboardEventArgs.Code)
		{
			case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
				InvokeOpenInEditor(commandArgs, true);
				return Task.CompletedTask;
			case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
				InvokeOpenInEditor(commandArgs, false);
				return Task.CompletedTask;
		}

		if (commandArgs.KeyboardEventArgs.CtrlKey)
		{
			CtrlModifiedKeymap(commandArgs);
			return Task.CompletedTask;
		}
		else if (commandArgs.KeyboardEventArgs.AltKey)
		{
			AltModifiedKeymap(commandArgs);
			return Task.CompletedTask;
		}

		return Task.CompletedTask;
	}

	private void CtrlModifiedKeymap(TreeViewCommandArgs commandArgs)
	{
		if (commandArgs.KeyboardEventArgs is null)
			return;

		if (commandArgs.KeyboardEventArgs.AltKey)
		{
			CtrlAltModifiedKeymap(commandArgs);
			return;
		}

		switch (commandArgs.KeyboardEventArgs.Key)
		{
			default:
				return;
		}
	}

	private void AltModifiedKeymap(TreeViewCommandArgs commandArgs)
	{
		return;
	}

	private void CtrlAltModifiedKeymap(TreeViewCommandArgs commandArgs)
	{
		return;
	}

	private void InvokeOpenInEditor(TreeViewCommandArgs commandArgs, bool shouldSetFocusToEditor)
	{
		return;
	}
}