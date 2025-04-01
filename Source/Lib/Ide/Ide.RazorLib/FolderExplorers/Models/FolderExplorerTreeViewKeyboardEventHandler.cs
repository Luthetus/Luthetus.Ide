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
using Luthetus.Ide.RazorLib.FolderExplorers.Displays;
using Luthetus.Ide.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.FolderExplorers.Models;

public class FolderExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly ITextEditorService _textEditorService;
    private readonly IMenuOptionsFactory _menuOptionsFactory;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly INotificationService _notificationService;

    public FolderExplorerTreeViewKeyboardEventHandler(
            IdeBackgroundTaskApi ideBackgroundTaskApi,
            ITextEditorService textEditorService,
            IMenuOptionsFactory menuOptionsFactory,
            ICommonComponentRenderers commonComponentRenderers,
            ITreeViewService treeViewService,
		    IBackgroundTaskService backgroundTaskService,
            IEnvironmentProvider environmentProvider,
            INotificationService notificationService)
        : base(treeViewService, backgroundTaskService)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _textEditorService = textEditorService;
        _menuOptionsFactory = menuOptionsFactory;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
        _environmentProvider = environmentProvider;
        _notificationService = notificationService;
    }

    public override Task OnKeyDownAsync(TreeViewCommandArgs commandArgs)
    {
        if (commandArgs.KeyboardEventArgs is null)
            return Task.CompletedTask;

        base.OnKeyDownAsync(commandArgs);

        switch (commandArgs.KeyboardEventArgs.Code)
        {
            case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
                return InvokeOpenInEditorAsync(commandArgs, true);
            case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                return InvokeOpenInEditorAsync(commandArgs, false);
        }

        if (commandArgs.KeyboardEventArgs.CtrlKey)
            CtrlModifiedKeymap(commandArgs);
        else if (commandArgs.KeyboardEventArgs.AltKey)
            AltModifiedKeymap(commandArgs);

        return Task.CompletedTask;
    }

    private void CtrlModifiedKeymap(TreeViewCommandArgs commandArgs)
    {
        if (commandArgs.KeyboardEventArgs is null)
            return;

        if (commandArgs.KeyboardEventArgs.AltKey)
        {
            CtrlAltModifiedKeymap(commandArgs);
        }
        else
        {
            switch (commandArgs.KeyboardEventArgs.Key)
            {
                case "c":
                    CopyFile(commandArgs);
                    return;
                case "x":
                    CutFile(commandArgs);
                    return;
                case "v":
                    PasteClipboard(commandArgs);
                    return;
            }
        }
    }

    /// <summary>
    /// Do not go from <see cref="AltModifiedKeymap" /> to <see cref="CtrlAltModifiedKeymap" />
    /// <br /><br />
    /// Code in this method should only be here if it does not include a Ctrl key being pressed.
    /// <br /><br />
    /// As otherwise, we'd have to permute over all the possible keyboard modifier keys and have a method for each permutation.
    /// </summary>
    private void AltModifiedKeymap(TreeViewCommandArgs commandArgs)
    {
        return;
    }

    private void CtrlAltModifiedKeymap(TreeViewCommandArgs commandArgs)
    {
        return;
    }

    private Task CopyFile(TreeViewCommandArgs commandArgs)
    {
        var activeNode = commandArgs.TreeViewContainer.ActiveNode;

        if (activeNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return Task.CompletedTask;

        var copyFileMenuOption = _menuOptionsFactory.CopyFile(
            treeViewAbsolutePath.Item,
            () =>
            {
                NotificationHelper.DispatchInformative("Copy Action", $"Copied: {treeViewAbsolutePath.Item.NameWithExtension}", _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(7));
                return Task.CompletedTask;
            });

        if (copyFileMenuOption.OnClickFunc is null)
            return Task.CompletedTask;

        return copyFileMenuOption.OnClickFunc.Invoke();
    }

    private Task PasteClipboard(TreeViewCommandArgs commandArgs)
    {
        var activeNode = commandArgs.TreeViewContainer.ActiveNode;

        if (activeNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return Task.CompletedTask;

        MenuOptionRecord pasteMenuOptionRecord;

        if (treeViewAbsolutePath.Item.IsDirectory)
        {
            pasteMenuOptionRecord = _menuOptionsFactory.PasteClipboard(
                treeViewAbsolutePath.Item,
                async () =>
                {
                    var localParentOfCutFile = FolderExplorerContextMenu.ParentOfCutFile;
                    FolderExplorerContextMenu.ParentOfCutFile = null;

                    if (localParentOfCutFile is not null)
                        await ReloadTreeViewModel(localParentOfCutFile).ConfigureAwait(false);

                    await ReloadTreeViewModel(treeViewAbsolutePath).ConfigureAwait(false);
                });
        }
        else
        {
            var parentDirectory = treeViewAbsolutePath.Item.ParentDirectory;

            var parentDirectoryAbsolutePath = _environmentProvider.AbsolutePathFactory(
                parentDirectory,
                true);

            pasteMenuOptionRecord = _menuOptionsFactory.PasteClipboard(
                parentDirectoryAbsolutePath,
                async () =>
                {
                    var localParentOfCutFile = FolderExplorerContextMenu.ParentOfCutFile;
                    FolderExplorerContextMenu.ParentOfCutFile = null;

                    if (localParentOfCutFile is not null)
                        await ReloadTreeViewModel(localParentOfCutFile).ConfigureAwait(false);

                    await ReloadTreeViewModel(treeViewAbsolutePath).ConfigureAwait(false);
                });
        }

        if (pasteMenuOptionRecord.OnClickFunc is null)
            return Task.CompletedTask;

        return pasteMenuOptionRecord.OnClickFunc.Invoke();
    }

    private Task CutFile(TreeViewCommandArgs commandArgs)
    {
        var activeNode = commandArgs.TreeViewContainer.ActiveNode;

        if (activeNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return Task.CompletedTask;

        var parent = treeViewAbsolutePath.Parent as TreeViewAbsolutePath;

        MenuOptionRecord cutFileOptionRecord = _menuOptionsFactory.CutFile(
            treeViewAbsolutePath.Item,
            () =>
            {
                FolderExplorerContextMenu.ParentOfCutFile = parent;
                NotificationHelper.DispatchInformative("Cut Action", $"Cut: {treeViewAbsolutePath.Item.NameWithExtension}", _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(7));
                return Task.CompletedTask;
            });

        if (cutFileOptionRecord.OnClickFunc is null)
            return Task.CompletedTask;

        return cutFileOptionRecord.OnClickFunc.Invoke();
    }

    private Task InvokeOpenInEditorAsync(TreeViewCommandArgs commandArgs, bool shouldSetFocusToEditor)
    {
        var activeNode = commandArgs.TreeViewContainer.ActiveNode;

        if (activeNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return Task.CompletedTask;

		_textEditorService.WorkerArbitrary.PostUnique(nameof(FolderExplorerTreeViewMouseEventHandler), async editContext =>
		{
			await _textEditorService.OpenInEditorAsync(
				editContext,
				treeViewAbsolutePath.Item.Value,
				shouldSetFocusToEditor,
				cursorPositionIndex: null,
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
            FolderExplorerState.TreeViewContentStateKey,
            treeViewModel);

        _treeViewService.ReduceMoveUpAction(
            FolderExplorerState.TreeViewContentStateKey,
            false,
			false);
    }
}