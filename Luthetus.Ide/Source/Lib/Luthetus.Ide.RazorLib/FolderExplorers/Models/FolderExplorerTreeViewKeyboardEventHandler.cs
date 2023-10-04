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

namespace Luthetus.Ide.RazorLib.FolderExplorers.Models;

public class FolderExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly EditorSync _editorSync;
    private readonly IMenuOptionsFactory _menuOptionsFactory;
    private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;

    public FolderExplorerTreeViewKeyboardEventHandler(
        EditorSync editorSync,
        IMenuOptionsFactory menuOptionsFactory,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService)
        : base(treeViewService)
    {
        _editorSync = editorSync;
        _menuOptionsFactory = menuOptionsFactory;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
    }

    public override Task OnKeyDownAsync(TreeViewCommandParameter commandParameter)
    {
        if (commandParameter.KeyboardEventArgs is null)
            return Task.CompletedTask;

        base.OnKeyDownAsync(commandParameter);

        switch (commandParameter.KeyboardEventArgs.Code)
        {
            case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
                InvokeOpenInEditor(commandParameter, true);
                return Task.CompletedTask;
            case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                InvokeOpenInEditor(commandParameter, false);
                return Task.CompletedTask;
        }

        if (commandParameter.KeyboardEventArgs.CtrlKey)
            CtrlModifiedKeymap(commandParameter);
        else if (commandParameter.KeyboardEventArgs.AltKey)
            AltModifiedKeymap(commandParameter);

        return Task.CompletedTask;
    }

    private void CtrlModifiedKeymap(TreeViewCommandParameter commandParameter)
    {
        if (commandParameter.KeyboardEventArgs is null)
            return;

        if (commandParameter.KeyboardEventArgs.AltKey)
        {
            CtrlAltModifiedKeymap(commandParameter);
        }
        else
        {
            switch (commandParameter.KeyboardEventArgs.Key)
            {
                case "c":
                    CopyFile(commandParameter);
                    return;
                case "x":
                    CutFile(commandParameter);
                    return;
                case "v":
                    PasteClipboard(commandParameter);
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
    private void AltModifiedKeymap(TreeViewCommandParameter commandParameter)
    {
        return;
    }

    private void CtrlAltModifiedKeymap(TreeViewCommandParameter commandParameter)
    {
        return;
    }

    private void CopyFile(TreeViewCommandParameter commandParameter)
    {
        var activeNode = commandParameter.TreeViewState.ActiveNode;

        if (activeNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        var copyFileMenuOption = _menuOptionsFactory.CopyFile(
            treeViewAbsolutePath.Item,
            () =>
            {
                NotificationHelper.DispatchInformative("Copy Action", $"Copied: {treeViewAbsolutePath.Item.NameWithExtension}", _commonComponentRenderers, _editorSync.Dispatcher);
                return Task.CompletedTask;
            });

        copyFileMenuOption.OnClick?.Invoke();
    }

    private Task PasteClipboard(TreeViewCommandParameter commandParameter)
    {
        var activeNode = commandParameter.TreeViewState.ActiveNode;

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
            var parentDirectory = treeViewAbsolutePath.Item.AncestorDirectoryBag.Last();

            pasteMenuOptionRecord = _menuOptionsFactory.PasteClipboard(
                parentDirectory,
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

    private void CutFile(TreeViewCommandParameter commandParameter)
    {
        var activeNode = commandParameter.TreeViewState.ActiveNode;

        if (activeNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        var parent = treeViewAbsolutePath.Parent as TreeViewAbsolutePath;

        MenuOptionRecord cutFileOptionRecord = _menuOptionsFactory.CutFile(
            treeViewAbsolutePath.Item,
            () =>
            {
                SolutionExplorerContextMenu.ParentOfCutFile = parent;
                NotificationHelper.DispatchInformative("Cut Action", $"Cut: {treeViewAbsolutePath.Item.NameWithExtension}", _commonComponentRenderers, _editorSync.Dispatcher);
                return Task.CompletedTask;
            });

        cutFileOptionRecord.OnClick?.Invoke();
    }

    private void InvokeOpenInEditor(TreeViewCommandParameter commandParameter, bool shouldSetFocusToEditor)
    {
        var activeNode = commandParameter.TreeViewState.ActiveNode;

        if (activeNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        _editorSync.OpenInEditor(treeViewAbsolutePath.Item, shouldSetFocusToEditor);
        return;
    }

    private async Task ReloadTreeViewModel(TreeViewNoType? treeViewModel)
    {
        if (treeViewModel is null)
            return;

        await treeViewModel.LoadChildBagAsync();

        _treeViewService.ReRenderNode(
            FolderExplorerState.TreeViewContentStateKey,
            treeViewModel);

        _treeViewService.MoveUp(
            FolderExplorerState.TreeViewContentStateKey,
            false);
    }
}