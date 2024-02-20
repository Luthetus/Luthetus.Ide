using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.Displays;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.Ide.RazorLib.FolderExplorers.States;
using Luthetus.Ide.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.Tests.Basis.FolderExplorers.Models;

public class FolderExplorerTreeViewKeyboardEventHandlerTests
{
    private readonly EditorSync _editorSync;
    private readonly IMenuOptionsFactory _menuOptionsFactory;
    private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IEnvironmentProvider _environmentProvider;

    public FolderExplorerTreeViewKeyboardEventHandler(
            EditorSync editorSync,
            IMenuOptionsFactory menuOptionsFactory,
            ILuthetusCommonComponentRenderers commonComponentRenderers,
            ITreeViewService treeViewService,
		    IBackgroundTaskService backgroundTaskService,
            IEnvironmentProvider environmentProvider)
        : base(treeViewService, backgroundTaskService)
    {
        _editorSync = editorSync;
        _menuOptionsFactory = menuOptionsFactory;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
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
                InvokeOpenInEditor(commandArgs, true);
                return Task.CompletedTask;
            case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                InvokeOpenInEditor(commandArgs, false);
                return Task.CompletedTask;
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

    private void CopyFile(TreeViewCommandArgs commandArgs)
    {
        var activeNode = commandArgs.TreeViewContainer.ActiveNode;

        if (activeNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        var copyFileMenuOption = _menuOptionsFactory.CopyFile(
            treeViewAbsolutePath.Item,
            () =>
            {
                NotificationHelper.DispatchInformative("Copy Action", $"Copied: {treeViewAbsolutePath.Item.NameWithExtension}", _commonComponentRenderers, _editorSync.Dispatcher, TimeSpan.FromSeconds(7));
                return Task.CompletedTask;
            });

        copyFileMenuOption.OnClick?.Invoke();
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
                    var localParentOfCutFile = SolutionExplorerContextMenu.ParentOfCutFile;
                    SolutionExplorerContextMenu.ParentOfCutFile = null;

                    if (localParentOfCutFile is not null)
                        await ReloadTreeViewModel(localParentOfCutFile);

                    await ReloadTreeViewModel(treeViewAbsolutePath);
                });
        }
        else
        {
            var parentDirectory = treeViewAbsolutePath.Item.AncestorDirectoryList.Last();

            var parentDirectoryAbsolutePath = _environmentProvider.AbsolutePathFactory(
                parentDirectory.Value,
                true);

            pasteMenuOptionRecord = _menuOptionsFactory.PasteClipboard(
                parentDirectoryAbsolutePath,
                async () =>
                {
                    var localParentOfCutFile = SolutionExplorerContextMenu.ParentOfCutFile;
                    SolutionExplorerContextMenu.ParentOfCutFile = null;

                    if (localParentOfCutFile is not null)
                        await ReloadTreeViewModel(localParentOfCutFile);

                    await ReloadTreeViewModel(treeViewAbsolutePath);
                });
        }

        pasteMenuOptionRecord.OnClick?.Invoke();
        return Task.CompletedTask;
    }

    private void CutFile(TreeViewCommandArgs commandArgs)
    {
        var activeNode = commandArgs.TreeViewContainer.ActiveNode;

        if (activeNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        var parent = treeViewAbsolutePath.Parent as TreeViewAbsolutePath;

        MenuOptionRecord cutFileOptionRecord = _menuOptionsFactory.CutFile(
            treeViewAbsolutePath.Item,
            () =>
            {
                SolutionExplorerContextMenu.ParentOfCutFile = parent;
                NotificationHelper.DispatchInformative("Cut Action", $"Cut: {treeViewAbsolutePath.Item.NameWithExtension}", _commonComponentRenderers, _editorSync.Dispatcher, TimeSpan.FromSeconds(7));
                return Task.CompletedTask;
            });

        cutFileOptionRecord.OnClick?.Invoke();
    }

    private void InvokeOpenInEditor(TreeViewCommandArgs commandArgs, bool shouldSetFocusToEditor)
    {
        var activeNode = commandArgs.TreeViewContainer.ActiveNode;

        if (activeNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        _editorSync.OpenInEditor(treeViewAbsolutePath.Item, shouldSetFocusToEditor);
        return;
    }

    private async Task ReloadTreeViewModel(TreeViewNoType? treeViewModel)
    {
        if (treeViewModel is null)
            return;

        await treeViewModel.LoadChildListAsync();

        _treeViewService.ReRenderNode(
            FolderExplorerState.TreeViewContentStateKey,
            treeViewModel);

        _treeViewService.MoveUp(
            FolderExplorerState.TreeViewContentStateKey,
            false,
			false);
    }
}