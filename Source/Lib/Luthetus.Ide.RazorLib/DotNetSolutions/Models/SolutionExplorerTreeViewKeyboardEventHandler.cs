using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.DotNetSolutions.Displays;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.Ide.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Models;

public class SolutionExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly EditorSync _editorSync;
    private readonly IMenuOptionsFactory _menuOptionsFactory;
    private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;

    public SolutionExplorerTreeViewKeyboardEventHandler(
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
        {
            CtrlModifiedKeymap(commandParameter);
            return Task.CompletedTask;
        }
        else if (commandParameter.KeyboardEventArgs.AltKey)
        {
            AltModifiedKeymap(commandParameter);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    private void CtrlModifiedKeymap(TreeViewCommandParameter commandParameter)
    {
        if (commandParameter.KeyboardEventArgs is null)
            return;

        if (commandParameter.KeyboardEventArgs.AltKey)
        {
            CtrlAltModifiedKeymap(commandParameter);
            return;
        }

        switch (commandParameter.KeyboardEventArgs.Key)
        {
            case "c":
                InvokeCopyFile(commandParameter);
                return;
            case "x":
                InvokeCutFile(commandParameter);
                return;
            case "v":
                InvokePasteClipboard(commandParameter);
                return;
        }
    }

    private void AltModifiedKeymap(TreeViewCommandParameter commandParameter)
    {
        return;
    }

    private void CtrlAltModifiedKeymap(TreeViewCommandParameter commandParameter)
    {
        return;
    }

    private void InvokeCopyFile(TreeViewCommandParameter commandParameter)
    {
        var activeNode = commandParameter.TreeViewState.ActiveNode;

        if (activeNode is not TreeViewNamespacePath treeViewNamespacePath)
            return;

        var copyFileMenuOption = _menuOptionsFactory.CopyFile(
            treeViewNamespacePath.Item.AbsolutePath,
            () =>
            {
                NotificationHelper.DispatchInformative("Copy Action", $"Copied: {treeViewNamespacePath.Item.AbsolutePath.NameWithExtension}", _commonComponentRenderers, _editorSync.Dispatcher);
                return Task.CompletedTask;
            });

        copyFileMenuOption.OnClick?.Invoke();
    }

    private void InvokePasteClipboard(TreeViewCommandParameter commandParameter)
    {
        var activeNode = commandParameter.TreeViewState.ActiveNode;

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
                .Item.AbsolutePath.AncestorDirectoryBag.Last();

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

    private void InvokeCutFile(TreeViewCommandParameter commandParameter)
    {
        var activeNode = commandParameter.TreeViewState.ActiveNode;

        if (activeNode is not TreeViewNamespacePath treeViewNamespacePath)
            return;

        var parent = treeViewNamespacePath.Parent as TreeViewNamespacePath;

        MenuOptionRecord cutFileOptionRecord = _menuOptionsFactory.CutFile(
            treeViewNamespacePath.Item.AbsolutePath,
            () =>
            {
                NotificationHelper.DispatchInformative("Cut Action", $"Cut: {treeViewNamespacePath.Item.AbsolutePath.NameWithExtension}", _commonComponentRenderers, _editorSync.Dispatcher);
                SolutionExplorerContextMenu.ParentOfCutFile = parent;
                return Task.CompletedTask;
            });

        cutFileOptionRecord.OnClick?.Invoke();
    }

    private void InvokeOpenInEditor(
        TreeViewCommandParameter commandParameter,
        bool shouldSetFocusToEditor)
    {
        var activeNode = commandParameter.TreeViewState.ActiveNode;

        if (activeNode is not TreeViewNamespacePath treeViewNamespacePath)
            return;

        _editorSync.OpenInEditor(treeViewNamespacePath.Item.AbsolutePath, shouldSetFocusToEditor);
        return;
    }

    private async Task ReloadTreeViewModel(
        TreeViewNoType? treeViewModel)
    {
        if (treeViewModel is null)
            return;

        await treeViewModel.LoadChildBagAsync();

        _treeViewService.ReRenderNode(
            DotNetSolutionState.TreeViewSolutionExplorerStateKey,
            treeViewModel);

        _treeViewService.MoveUp(
            DotNetSolutionState.TreeViewSolutionExplorerStateKey,
            false);
    }
}