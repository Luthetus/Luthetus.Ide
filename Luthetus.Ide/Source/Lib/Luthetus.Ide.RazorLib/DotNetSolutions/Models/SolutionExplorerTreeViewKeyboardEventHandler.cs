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
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Models;

public class SolutionExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly EditorSync _editorSync;
    private readonly IMenuOptionsFactory _menuOptionsFactory;
    private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IEnvironmentProvider _environmentProvider;

    public SolutionExplorerTreeViewKeyboardEventHandler(
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

    private void InvokeCopyFile(TreeViewCommandArgs commandArgs)
    {
        var activeNode = commandArgs.TreeViewContainer.ActiveNode;

        if (activeNode is not TreeViewNamespacePath treeViewNamespacePath)
            return;

        var copyFileMenuOption = _menuOptionsFactory.CopyFile(
            treeViewNamespacePath.Item.AbsolutePath,
            () =>
            {
                NotificationHelper.DispatchInformative("Copy Action", $"Copied: {treeViewNamespacePath.Item.AbsolutePath.NameWithExtension}", _commonComponentRenderers, _editorSync.Dispatcher, TimeSpan.FromSeconds(7));
                return Task.CompletedTask;
            });

        copyFileMenuOption.OnClick?.Invoke();
    }

    private void InvokePasteClipboard(TreeViewCommandArgs commandArgs)
    {
        var activeNode = commandArgs.TreeViewContainer.ActiveNode;

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
                .Item.AbsolutePath.AncestorDirectoryList.Last();

            var parentDirectoryAbsolutePath = _environmentProvider.AbsolutePathFactory(parentDirectory.Path, true);

            pasteMenuOptionRecord = _menuOptionsFactory.PasteClipboard(
                parentDirectoryAbsolutePath,
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

    private void InvokeCutFile(TreeViewCommandArgs commandArgs)
    {
        var activeNode = commandArgs.TreeViewContainer.ActiveNode;

        if (activeNode is not TreeViewNamespacePath treeViewNamespacePath)
            return;

        var parent = treeViewNamespacePath.Parent as TreeViewNamespacePath;

        MenuOptionRecord cutFileOptionRecord = _menuOptionsFactory.CutFile(
            treeViewNamespacePath.Item.AbsolutePath,
            () =>
            {
                NotificationHelper.DispatchInformative("Cut Action", $"Cut: {treeViewNamespacePath.Item.AbsolutePath.NameWithExtension}", _commonComponentRenderers, _editorSync.Dispatcher, TimeSpan.FromSeconds(7));
                SolutionExplorerContextMenu.ParentOfCutFile = parent;
                return Task.CompletedTask;
            });

        cutFileOptionRecord.OnClick?.Invoke();
    }

    private void InvokeOpenInEditor(TreeViewCommandArgs commandArgs, bool shouldSetFocusToEditor)
    {
        var activeNode = commandArgs.TreeViewContainer.ActiveNode;

        if (activeNode is not TreeViewNamespacePath treeViewNamespacePath)
            return;

        _editorSync.OpenInEditor(treeViewNamespacePath.Item.AbsolutePath, shouldSetFocusToEditor);
        return;
    }

    private async Task ReloadTreeViewModel(TreeViewNoType? treeViewModel)
    {
        if (treeViewModel is null)
            return;

        await treeViewModel.LoadChildListAsync();

        _treeViewService.ReRenderNode(
            DotNetSolutionState.TreeViewSolutionExplorerStateKey,
            treeViewModel);

        _treeViewService.MoveUp(
            DotNetSolutionState.TreeViewSolutionExplorerStateKey,
            false,
            false);
    }
}