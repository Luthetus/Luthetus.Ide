using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.FileSystem.Classes.LuthetusPath;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Keyboard;
using Luthetus.Common.RazorLib.Menu;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.MenuCase;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.Ide.ClassLib.FolderExplorerCase;
using Luthetus.Ide.ClassLib.TreeViewImplementationsCase;
using Luthetus.Ide.RazorLib.SolutionExplorer;

namespace Luthetus.Ide.RazorLib.FolderExplorer.Classes;

public class FolderExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly IMenuOptionsFactory _menuOptionsFactory;
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
    private readonly IDispatcher _dispatcher;
    private readonly ITreeViewService _treeViewService;

    public FolderExplorerTreeViewKeyboardEventHandler(
        IMenuOptionsFactory menuOptionsFactory,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        IDispatcher dispatcher,
        ITreeViewService treeViewService)
        : base(treeViewService)
    {
        _menuOptionsFactory = menuOptionsFactory;
        _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
        _dispatcher = dispatcher;
        _treeViewService = treeViewService;
    }

    public override void OnKeyDown(ITreeViewCommandParameter treeViewCommandParameter)
    {
        if (treeViewCommandParameter.KeyboardEventArgs is null)
            return;

        base.OnKeyDown(treeViewCommandParameter);

        switch (treeViewCommandParameter.KeyboardEventArgs.Code)
        {
            case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
                InvokeOpenInEditor(treeViewCommandParameter, true);
                return;
            case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                InvokeOpenInEditor(treeViewCommandParameter, false);
                return;
        }

        if (treeViewCommandParameter.KeyboardEventArgs.CtrlKey)
            CtrlModifiedKeymap(treeViewCommandParameter);
        else if (treeViewCommandParameter.KeyboardEventArgs.AltKey)
            AltModifiedKeymap(treeViewCommandParameter);
    }

    private void CtrlModifiedKeymap(ITreeViewCommandParameter treeViewCommandParameter)
    {
        if (treeViewCommandParameter.KeyboardEventArgs is null)
            return;

        if (treeViewCommandParameter.KeyboardEventArgs.AltKey)
        {
            CtrlAltModifiedKeymap(treeViewCommandParameter);
        }
        else
        {
            switch (treeViewCommandParameter.KeyboardEventArgs.Key)
            {
                case "c":
                    CopyFile(treeViewCommandParameter);
                    return;
                case "x":
                    CutFile(treeViewCommandParameter);
                    return;
                case "v":
                    PasteClipboard(treeViewCommandParameter);
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
    private void AltModifiedKeymap(ITreeViewCommandParameter treeViewCommandParameter)
    {
        return;
    }

    private void CtrlAltModifiedKeymap(ITreeViewCommandParameter treeViewCommandParameter)
    {
        return;
    }

    private void CopyFile(ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        var copyFileMenuOption = _menuOptionsFactory.CopyFile(
            treeViewAbsolutePath.Item,
            () => { 
                NotificationHelper.DispatchInformative("Copy Action", $"Copied: {treeViewAbsolutePath.Item.NameWithExtension}", _luthetusCommonComponentRenderers, _dispatcher);
                return Task.CompletedTask;
            });

        copyFileMenuOption.OnClick?.Invoke();
    }

    private Task PasteClipboard(ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return Task.CompletedTask;

        MenuOptionRecord pasteMenuOptionRecord;

        if (treeViewAbsolutePath.Item.IsDirectory)
        {
            pasteMenuOptionRecord = _menuOptionsFactory.PasteClipboard(
                treeViewAbsolutePath.Item,
                async () =>
                {
                    var localParentOfCutFile =
                        SolutionExplorerContextMenu.ParentOfCutFile;

                    SolutionExplorerContextMenu.ParentOfCutFile = null;

                    if (localParentOfCutFile is not null)
                        await ReloadTreeViewModel(localParentOfCutFile);

                    await ReloadTreeViewModel(treeViewAbsolutePath);
                });
        }
        else
        {
            var parentDirectory = (IAbsolutePath)treeViewAbsolutePath
                .Item.AncestorDirectories.Last();

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

    private void CutFile(ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        var parent = treeViewAbsolutePath.Parent as TreeViewAbsolutePath;

        MenuOptionRecord cutFileOptionRecord = _menuOptionsFactory.CutFile(
            treeViewAbsolutePath.Item,
            () => {
                SolutionExplorerContextMenu.ParentOfCutFile = parent;
                NotificationHelper.DispatchInformative("Cut Action", $"Cut: {treeViewAbsolutePath.Item.NameWithExtension}", _luthetusCommonComponentRenderers, _dispatcher);
                return Task.CompletedTask;
            });

        cutFileOptionRecord.OnClick?.Invoke();
    }

    private void InvokeOpenInEditor(
        ITreeViewCommandParameter treeViewCommandParameter,
        bool shouldSetFocusToEditor)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        _dispatcher.Dispatch(new EditorRegistry.OpenInEditorAction(
            treeViewAbsolutePath.Item,
            shouldSetFocusToEditor));

        return;
    }

    private async Task ReloadTreeViewModel(TreeViewNoType? treeViewModel)
    {
        if (treeViewModel is null)
            return;

        await treeViewModel.LoadChildrenAsync();

        _treeViewService.ReRenderNode(
            FolderExplorerRegistry.TreeViewFolderExplorerContentStateKey,
            treeViewModel);

        _treeViewService.MoveUp(
            FolderExplorerRegistry.TreeViewFolderExplorerContentStateKey,
            false);
    }
}