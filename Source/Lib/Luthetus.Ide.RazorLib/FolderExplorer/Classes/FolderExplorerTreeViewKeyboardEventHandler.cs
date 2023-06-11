using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.Keyboard;
using Luthetus.Common.RazorLib.Menu;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Menu;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Ide.RazorLib.SolutionExplorer;
using Fluxor;
using Luthetus.Ide.ClassLib.Store.FolderExplorerCase;

namespace Luthetus.Ide.RazorLib.FolderExplorer.Classes;

public class FolderExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly ICommonMenuOptionsFactory _commonMenuOptionsFactory;
    private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
    private readonly IDispatcher _dispatcher;
    private readonly ITreeViewService _treeViewService;

    public FolderExplorerTreeViewKeyboardEventHandler(
        ICommonMenuOptionsFactory commonMenuOptionsFactory,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        IDispatcher dispatcher,
        ITreeViewService treeViewService)
        : base(treeViewService)
    {
        _commonMenuOptionsFactory = commonMenuOptionsFactory;
        _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        _dispatcher = dispatcher;
        _treeViewService = treeViewService;
    }

    public override async Task<bool> OnKeyDownAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        if (treeViewCommandParameter.KeyboardEventArgs is null)
            return false;

        _ = await base.OnKeyDownAsync(treeViewCommandParameter);

        switch (treeViewCommandParameter.KeyboardEventArgs.Code)
        {
            case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
                await InvokeOpenInEditorAsync(
                    treeViewCommandParameter,
                    true);
                return true;
            case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                await InvokeOpenInEditorAsync(
                    treeViewCommandParameter,
                    false);
                return true;
        }

        if (treeViewCommandParameter.KeyboardEventArgs.CtrlKey)
        {
            var wasMappedToAnAction = await CtrlModifiedKeymapAsync(treeViewCommandParameter);

            if (wasMappedToAnAction)
                return wasMappedToAnAction;
        }

        if (treeViewCommandParameter.KeyboardEventArgs.AltKey)
        {
            var wasMappedToAnAction = await AltModifiedKeymapAsync(treeViewCommandParameter);

            if (wasMappedToAnAction)
                return wasMappedToAnAction;
        }

        return false;
    }

    private async Task<bool> CtrlModifiedKeymapAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        if (treeViewCommandParameter.KeyboardEventArgs is null)
            return false;

        if (treeViewCommandParameter.KeyboardEventArgs.AltKey)
        {
            var wasMappedToAnAction = await CtrlAltModifiedKeymapAsync(treeViewCommandParameter);

            if (wasMappedToAnAction)
                return wasMappedToAnAction;
        }

        switch (treeViewCommandParameter.KeyboardEventArgs.Key)
        {
            case "c":
                await InvokeCopyFileAsync(treeViewCommandParameter);
                return true;
            case "x":
                await InvokeCutFile(treeViewCommandParameter);
                return true;
            case "v":
                await InvokePasteClipboard(treeViewCommandParameter);
                return true;
        }

        return false;
    }

    /// <summary>
    ///     Do not go from <see cref="AltModifiedKeymapAsync" /> to
    ///     <see cref="CtrlAltModifiedKeymapAsync" />
    ///     <br /><br />
    ///     Code in this method should only be here if it
    ///     does not include a Ctrl key being pressed.
    ///     <br /><br />
    ///     As otherwise, we'd have to permute over
    ///     all the possible keyboard modifier
    ///     keys and have a method for each permutation.
    /// </summary>
    private Task<bool> AltModifiedKeymapAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        return Task.FromResult(false);
    }

    private Task<bool> CtrlAltModifiedKeymapAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        return Task.FromResult(false);
    }

    private Task NotifyCopyCompleted(IAbsoluteFilePath absoluteFilePath)
    {
        if (_luthetusIdeComponentRenderers.LuthetusCommonComponentRenderers.InformativeNotificationRendererType is not null)
        {
            var notificationInformative = new NotificationRecord(
                NotificationKey.NewNotificationKey(),
                "Copy Action",
                _luthetusIdeComponentRenderers.LuthetusCommonComponentRenderers.InformativeNotificationRendererType,
                new Dictionary<string, object?>
                {
                    {
                        nameof(IInformativeNotificationRendererType.Message),
                        $"Copied: {absoluteFilePath.FilenameWithExtension}"
                    },
                },
                TimeSpan.FromSeconds(3),
                null);

            _dispatcher.Dispatch(
                new NotificationRecordsCollection.RegisterAction(
                    notificationInformative));
        }

        return Task.CompletedTask;
    }

    private Task NotifyCutCompleted(
        IAbsoluteFilePath absoluteFilePath,
        TreeViewAbsoluteFilePath? parentTreeViewModel)
    {
        SolutionExplorerContextMenu.ParentOfCutFile = parentTreeViewModel;

        if (_luthetusIdeComponentRenderers.LuthetusCommonComponentRenderers.InformativeNotificationRendererType is not null)
        {
            var notificationInformative = new NotificationRecord(
                NotificationKey.NewNotificationKey(),
                "Cut Action",
                _luthetusIdeComponentRenderers.LuthetusCommonComponentRenderers.InformativeNotificationRendererType,
                new Dictionary<string, object?>
                {
                    {
                        nameof(IInformativeNotificationRendererType.Message),
                        $"Cut: {absoluteFilePath.FilenameWithExtension}"
                    },
                },
                TimeSpan.FromSeconds(3),
                null);

            _dispatcher.Dispatch(
                new NotificationRecordsCollection.RegisterAction(
                    notificationInformative));
        }

        return Task.CompletedTask;
    }

    private Task InvokeCopyFileAsync(ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is null ||
            activeNode is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePathPath ||
            treeViewAbsoluteFilePathPath.Item is null)
        {
            return Task.CompletedTask;
        }

        var copyFileMenuOption = _commonMenuOptionsFactory.CopyFile(
            treeViewAbsoluteFilePathPath.Item,
            () => NotifyCopyCompleted(treeViewAbsoluteFilePathPath.Item));

        copyFileMenuOption.OnClick?.Invoke();

        return Task.CompletedTask;
    }

    private Task InvokePasteClipboard(ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is null ||
            activeNode is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePathPath ||
            treeViewAbsoluteFilePathPath.Item is null)
        {
            return Task.CompletedTask;
        }

        MenuOptionRecord pasteMenuOptionRecord;

        if (treeViewAbsoluteFilePathPath.Item.IsDirectory)
        {
            pasteMenuOptionRecord = _commonMenuOptionsFactory.PasteClipboard(
                treeViewAbsoluteFilePathPath.Item,
                async () =>
                {
                    var localParentOfCutFile =
                        SolutionExplorerContextMenu.ParentOfCutFile;

                    SolutionExplorerContextMenu.ParentOfCutFile = null;

                    if (localParentOfCutFile is not null)
                        await ReloadTreeViewModel(localParentOfCutFile);

                    await ReloadTreeViewModel(treeViewAbsoluteFilePathPath);
                });
        }
        else
        {
            var parentDirectory = (IAbsoluteFilePath)treeViewAbsoluteFilePathPath
                .Item.Directories.Last();

            pasteMenuOptionRecord = _commonMenuOptionsFactory.PasteClipboard(
                parentDirectory,
                async () =>
                {
                    var localParentOfCutFile =
                        SolutionExplorerContextMenu.ParentOfCutFile;

                    SolutionExplorerContextMenu.ParentOfCutFile = null;

                    if (localParentOfCutFile is not null)
                        await ReloadTreeViewModel(localParentOfCutFile);

                    await ReloadTreeViewModel(treeViewAbsoluteFilePathPath);
                });
        }

        pasteMenuOptionRecord.OnClick?.Invoke();
        return Task.CompletedTask;
    }

    private Task InvokeCutFile(ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is null ||
            activeNode is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePathPath ||
            treeViewAbsoluteFilePathPath.Item is null)
        {
            return Task.CompletedTask;
        }

        var parent = treeViewAbsoluteFilePathPath.Parent as TreeViewAbsoluteFilePath;

        MenuOptionRecord cutFileOptionRecord = _commonMenuOptionsFactory.CutFile(
            treeViewAbsoluteFilePathPath.Item,
            () => NotifyCutCompleted(
                treeViewAbsoluteFilePathPath.Item,
                parent));

        cutFileOptionRecord.OnClick?.Invoke();
        return Task.CompletedTask;
    }

    private Task InvokeOpenInEditorAsync(
        ITreeViewCommandParameter treeViewCommandParameter,
        bool shouldSetFocusToEditor)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is null ||
            activeNode is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePathPath ||
            treeViewAbsoluteFilePathPath.Item is null)
        {
            return Task.CompletedTask;
        }

        _dispatcher.Dispatch(new EditorState.OpenInEditorAction(
            treeViewAbsoluteFilePathPath.Item,
            shouldSetFocusToEditor));

        return Task.CompletedTask;
    }

    private async Task ReloadTreeViewModel(
        TreeViewNoType? treeViewModel)
    {
        if (treeViewModel is null)
            return;

        await treeViewModel.LoadChildrenAsync();

        _treeViewService.ReRenderNode(
            FolderExplorerState.TreeViewFolderExplorerContentStateKey,
            treeViewModel);

        _treeViewService.MoveUp(
            FolderExplorerState.TreeViewFolderExplorerContentStateKey,
            false);
    }
}