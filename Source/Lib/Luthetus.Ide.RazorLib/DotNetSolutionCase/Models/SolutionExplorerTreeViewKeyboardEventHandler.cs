using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keyboard.Models;
using Luthetus.Common.RazorLib.Menu.Models;
using Luthetus.Common.RazorLib.Notification.Models;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.Displays;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.States;
using Luthetus.Ide.RazorLib.EditorCase.States;
using Luthetus.Ide.RazorLib.MenuCase.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.Models;

public class SolutionExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly EditorSync _editorSync;
    private readonly IMenuOptionsFactory _menuOptionsFactory;
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
    private readonly ITreeViewService _treeViewService;

    public SolutionExplorerTreeViewKeyboardEventHandler(
        EditorSync editorSync,
        IMenuOptionsFactory menuOptionsFactory,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        ITreeViewService treeViewService)
        : base(treeViewService)
    {
        _editorSync = editorSync;
        _menuOptionsFactory = menuOptionsFactory;
        _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
        _treeViewService = treeViewService;
    }

    public override Task OnKeyDownAsync(TreeViewCommandParameter treeViewCommandParameter)
    {
        if (treeViewCommandParameter.KeyboardEventArgs is null)
            return Task.CompletedTask;

        base.OnKeyDownAsync(treeViewCommandParameter);

        switch (treeViewCommandParameter.KeyboardEventArgs.Code)
        {
            case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
                InvokeOpenInEditor(treeViewCommandParameter, true);
                return Task.CompletedTask;
            case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                InvokeOpenInEditor(treeViewCommandParameter, false);
                return Task.CompletedTask;
        }

        if (treeViewCommandParameter.KeyboardEventArgs.CtrlKey)
        {
            CtrlModifiedKeymap(treeViewCommandParameter);
            return Task.CompletedTask;
        }
        else if (treeViewCommandParameter.KeyboardEventArgs.AltKey)
        {
            AltModifiedKeymap(treeViewCommandParameter);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    private void CtrlModifiedKeymap(TreeViewCommandParameter treeViewCommandParameter)
    {
        if (treeViewCommandParameter.KeyboardEventArgs is null)
            return;

        if (treeViewCommandParameter.KeyboardEventArgs.AltKey)
        {
            CtrlAltModifiedKeymap(treeViewCommandParameter);
            return;
        }

        switch (treeViewCommandParameter.KeyboardEventArgs.Key)
        {
            case "c":
                InvokeCopyFile(treeViewCommandParameter);
                return;
            case "x":
                InvokeCutFile(treeViewCommandParameter);
                return;
            case "v":
                InvokePasteClipboard(treeViewCommandParameter);
                return;
        }
    }

    private void AltModifiedKeymap(TreeViewCommandParameter treeViewCommandParameter)
    {
        return;
    }

    private void CtrlAltModifiedKeymap(TreeViewCommandParameter treeViewCommandParameter)
    {
        return;
    }

    private void InvokeCopyFile(TreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is not TreeViewNamespacePath treeViewNamespacePath)
            return;

        var copyFileMenuOption = _menuOptionsFactory.CopyFile(
            treeViewNamespacePath.Item.AbsolutePath,
            () =>
            {
                NotificationHelper.DispatchInformative("Copy Action", $"Copied: {treeViewNamespacePath.Item.AbsolutePath.NameWithExtension}", _luthetusCommonComponentRenderers, _editorSync.Dispatcher);
                return Task.CompletedTask;
            });

        copyFileMenuOption.OnClick?.Invoke();
    }

    private void InvokePasteClipboard(TreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is not TreeViewNamespacePath treeViewNamespacePath)
            return;

        MenuOptionRecord pasteMenuOptionRecord;

        if (treeViewNamespacePath.Item.AbsolutePath.IsDirectory)
        {
            pasteMenuOptionRecord = _menuOptionsFactory.PasteClipboard(
                treeViewNamespacePath.Item.AbsolutePath,
                async () =>
                {
                    var localParentOfCutFile =
                        SolutionExplorerContextMenu.ParentOfCutFile;

                    SolutionExplorerContextMenu.ParentOfCutFile = null;

                    if (localParentOfCutFile is not null)
                        await ReloadTreeViewModel(localParentOfCutFile);

                    await ReloadTreeViewModel(treeViewNamespacePath);
                });
        }
        else
        {
            var parentDirectory = treeViewNamespacePath
                .Item.AbsolutePath.AncestorDirectories.Last();

            pasteMenuOptionRecord = _menuOptionsFactory.PasteClipboard(
                parentDirectory,
                async () =>
                {
                    var localParentOfCutFile =
                        SolutionExplorerContextMenu.ParentOfCutFile;

                    SolutionExplorerContextMenu.ParentOfCutFile = null;

                    if (localParentOfCutFile is not null)
                        await ReloadTreeViewModel(localParentOfCutFile);

                    await ReloadTreeViewModel(treeViewNamespacePath);
                });
        }

        pasteMenuOptionRecord.OnClick?.Invoke();
    }

    private void InvokeCutFile(TreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is not TreeViewNamespacePath treeViewNamespacePath)
            return;

        var parent = treeViewNamespacePath.Parent as TreeViewNamespacePath;

        MenuOptionRecord cutFileOptionRecord = _menuOptionsFactory.CutFile(
            treeViewNamespacePath.Item.AbsolutePath,
            () =>
            {
                NotificationHelper.DispatchInformative("Cut Action", $"Cut: {treeViewNamespacePath.Item.AbsolutePath.NameWithExtension}", _luthetusCommonComponentRenderers, _editorSync.Dispatcher);
                SolutionExplorerContextMenu.ParentOfCutFile = parent;
                return Task.CompletedTask;
            });

        cutFileOptionRecord.OnClick?.Invoke();
    }

    private void InvokeOpenInEditor(
        TreeViewCommandParameter treeViewCommandParameter,
        bool shouldSetFocusToEditor)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is not TreeViewNamespacePath treeViewNamespacePath)
            return;

        _editorSync.Dispatcher.Dispatch(new EditorState.OpenInEditorAction(
            _editorSync,
            treeViewNamespacePath.Item.AbsolutePath,
            shouldSetFocusToEditor));

        return;
    }

    private async Task ReloadTreeViewModel(
        TreeViewNoType? treeViewModel)
    {
        if (treeViewModel is null)
            return;

        await treeViewModel.LoadChildrenAsync();

        _treeViewService.ReRenderNode(
            DotNetSolutionState.TreeViewSolutionExplorerStateKey,
            treeViewModel);

        _treeViewService.MoveUp(
            DotNetSolutionState.TreeViewSolutionExplorerStateKey,
            false);
    }
}