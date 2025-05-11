using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Menus.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Displays.Internals;
using Luthetus.Extensions.DotNet.Namespaces.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Models;

public class SolutionExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly IMenuOptionsFactory _menuOptionsFactory;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly TextEditorService _textEditorService;
	private readonly ITreeViewService _treeViewService;
	private readonly INotificationService _notificationService;
	private readonly IEnvironmentProvider _environmentProvider;

	public SolutionExplorerTreeViewKeyboardEventHandler(
			IdeBackgroundTaskApi ideBackgroundTaskApi,
			IMenuOptionsFactory menuOptionsFactory,
			ICommonComponentRenderers commonComponentRenderers,
			TextEditorService textEditorService,
			ITreeViewService treeViewService,
			INotificationService notificationService,
			IBackgroundTaskService backgroundTaskService,
			IEnvironmentProvider environmentProvider)
		: base(treeViewService, backgroundTaskService)
	{
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
		_menuOptionsFactory = menuOptionsFactory;
		_commonComponentRenderers = commonComponentRenderers;
		_textEditorService = textEditorService;
		_treeViewService = treeViewService;
		_notificationService = notificationService;
		_environmentProvider = environmentProvider;
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
			case "c":
				InvokeCopyFile(commandArgs);
				return;
			case "x":
				InvokeCutFile(commandArgs);
				return;
			case "v":
				InvokePasteClipboard(commandArgs);
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

	private Task InvokeCopyFile(TreeViewCommandArgs commandArgs)
	{
		var activeNode = commandArgs.TreeViewContainer.ActiveNode;

		if (activeNode is not TreeViewNamespacePath treeViewNamespacePath)
			return Task.CompletedTask;

		var copyFileMenuOption = _menuOptionsFactory.CopyFile(
			treeViewNamespacePath.Item.AbsolutePath,
			() =>
			{
				NotificationHelper.DispatchInformative("Copy Action", $"Copied: {treeViewNamespacePath.Item.AbsolutePath.NameWithExtension}", _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(7));
				return Task.CompletedTask;
			});

		if (copyFileMenuOption.OnClickFunc is null)
			return Task.CompletedTask;

		return copyFileMenuOption.OnClickFunc.Invoke();
	}

	private Task InvokePasteClipboard(TreeViewCommandArgs commandArgs)
	{
		var activeNode = commandArgs.TreeViewContainer.ActiveNode;

		if (activeNode is not TreeViewNamespacePath treeViewNamespacePath)
			return Task.CompletedTask;

		MenuOptionRecord pasteMenuOptionRecord;

		if (treeViewNamespacePath.Item.AbsolutePath.IsDirectory)
		{
			pasteMenuOptionRecord = _menuOptionsFactory.PasteClipboard(
				treeViewNamespacePath.Item.AbsolutePath,
				async () =>
				{
					var localParentOfCutFile = SolutionExplorerContextMenu.ParentOfCutFile;
					SolutionExplorerContextMenu.ParentOfCutFile = null;

					if (localParentOfCutFile is not null)
						await ReloadTreeViewModel(localParentOfCutFile).ConfigureAwait(false);

					await ReloadTreeViewModel(treeViewNamespacePath).ConfigureAwait(false);
				});
		}
		else
		{
			var parentDirectory = treeViewNamespacePath.Item.AbsolutePath.ParentDirectory;
			var parentDirectoryAbsolutePath = _environmentProvider.AbsolutePathFactory(parentDirectory, true);

			pasteMenuOptionRecord = _menuOptionsFactory.PasteClipboard(
				parentDirectoryAbsolutePath,
				async () =>
				{
					var localParentOfCutFile = SolutionExplorerContextMenu.ParentOfCutFile;
					SolutionExplorerContextMenu.ParentOfCutFile = null;

					if (localParentOfCutFile is not null)
						await ReloadTreeViewModel(localParentOfCutFile).ConfigureAwait(false);

					await ReloadTreeViewModel(treeViewNamespacePath).ConfigureAwait(false);
				});
		}

		if (pasteMenuOptionRecord.OnClickFunc is null)
			return Task.CompletedTask;

		return pasteMenuOptionRecord.OnClickFunc.Invoke();
	}

	private Task InvokeCutFile(TreeViewCommandArgs commandArgs)
	{
		var activeNode = commandArgs.TreeViewContainer.ActiveNode;

		if (activeNode is not TreeViewNamespacePath treeViewNamespacePath)
			return Task.CompletedTask;

		var parent = treeViewNamespacePath.Parent as TreeViewNamespacePath;

		MenuOptionRecord cutFileOptionRecord = _menuOptionsFactory.CutFile(
			treeViewNamespacePath.Item.AbsolutePath,
			() =>
			{
				NotificationHelper.DispatchInformative("Cut Action", $"Cut: {treeViewNamespacePath.Item.AbsolutePath.NameWithExtension}", _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(7));
				SolutionExplorerContextMenu.ParentOfCutFile = parent;
				return Task.CompletedTask;
			});

		if (cutFileOptionRecord.OnClickFunc is null)
			return Task.CompletedTask;

		return cutFileOptionRecord.OnClickFunc.Invoke();
	}

	private Task InvokeOpenInEditor(TreeViewCommandArgs commandArgs, bool shouldSetFocusToEditor)
	{
		if (commandArgs.TreeViewContainer.ActiveNode is not TreeViewNamespacePath treeViewNamespacePath)
			return Task.CompletedTask;
			
		_textEditorService.WorkerArbitrary.PostUnique(nameof(SolutionExplorerTreeViewMouseEventHandler), async editContext =>
		{
			await _textEditorService.OpenInEditorAsync(
				editContext,
				treeViewNamespacePath.Item.AbsolutePath.Value,
				shouldSetFocusToEditor,
				null,
				new Category("main"),
				Key<TextEditorViewModel>.NewKey());
		});
	    return Task.CompletedTask;
	}

	private async Task ReloadTreeViewModel(TreeViewNoType? treeViewModel)
	{
		if (treeViewModel is null)
			return;

		await treeViewModel.LoadChildListAsync().ConfigureAwait(false);

		_treeViewService.ReduceReRenderNodeAction(
			DotNetSolutionState.TreeViewSolutionExplorerStateKey,
			treeViewModel);

		_treeViewService.ReduceMoveUpAction(
			DotNetSolutionState.TreeViewSolutionExplorerStateKey,
			false,
			false);
	}
}