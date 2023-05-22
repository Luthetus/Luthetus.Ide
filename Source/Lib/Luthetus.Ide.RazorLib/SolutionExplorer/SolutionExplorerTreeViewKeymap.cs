using Luthetus.Common.RazorLib.BackgroundTaskCase;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.Keyboard;
using Luthetus.Common.RazorLib.Menu;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Menu;
using Luthetus.Ide.ClassLib.Namespaces;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Fluxor;

namespace Luthetus.Ide.RazorLib.SolutionExplorer;

public class SolutionExplorerTreeViewKeymap : TreeViewKeyboardEventHandler
{
    private readonly IState<TerminalSessionsState> _terminalSessionsStateWrap;
    private readonly ICommonMenuOptionsFactory _commonMenuOptionsFactory;
    private readonly ILuthetusIdeComponentRenderers _blazorStudioComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IDispatcher _dispatcher;
    private readonly ITreeViewService _treeViewService;
    private readonly ITextEditorService _textEditorService;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;

    public SolutionExplorerTreeViewKeymap(
        IState<TerminalSessionsState> terminalSessionsStateWrap,
        ICommonMenuOptionsFactory commonMenuOptionsFactory,
        ILuthetusIdeComponentRenderers blazorStudioComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IDispatcher dispatcher,
        ITreeViewService treeViewService,
        ITextEditorService textEditorService,
        IBackgroundTaskQueue backgroundTaskQueue)
        : base(treeViewService)
    {
        _terminalSessionsStateWrap = terminalSessionsStateWrap;
        _commonMenuOptionsFactory = commonMenuOptionsFactory;
        _blazorStudioComponentRenderers = blazorStudioComponentRenderers;
        _fileSystemProvider = fileSystemProvider;
        _dispatcher = dispatcher;
        _treeViewService = treeViewService;
        _textEditorService = textEditorService;
        _backgroundTaskQueue = backgroundTaskQueue;
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

    private Task NotifyCopyCompleted(NamespacePath namespacePath)
    {
        if (_blazorStudioComponentRenderers.LuthetusCommonComponentRenderers.InformativeNotificationRendererType != null)
        {
            var notificationInformative = new NotificationRecord(
                NotificationKey.NewNotificationKey(),
                "Copy Action",
                _blazorStudioComponentRenderers.LuthetusCommonComponentRenderers.InformativeNotificationRendererType,
                new Dictionary<string, object?>
                {
                    {
                        nameof(IInformativeNotificationRendererType.Message),
                        $"Copied: {namespacePath.AbsoluteFilePath.FilenameWithExtension}"
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
        NamespacePath namespacePath,
        TreeViewNamespacePath? parentTreeViewModel)
    {
        SolutionExplorerContextMenu.ParentOfCutFile = parentTreeViewModel;

        if (_blazorStudioComponentRenderers.LuthetusCommonComponentRenderers.InformativeNotificationRendererType != null)
        {
            var notificationInformative = new NotificationRecord(
                NotificationKey.NewNotificationKey(),
                "Cut Action",
                _blazorStudioComponentRenderers.LuthetusCommonComponentRenderers.InformativeNotificationRendererType,
                new Dictionary<string, object?>
                {
                    {
                        nameof(IInformativeNotificationRendererType.Message),
                        $"Cut: {namespacePath.AbsoluteFilePath.FilenameWithExtension}"
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
            activeNode is not TreeViewNamespacePath treeViewNamespacePath ||
            treeViewNamespacePath.Item is null)
        {
            return Task.CompletedTask;
        }

        var copyFileMenuOption = _commonMenuOptionsFactory.CopyFile(
            treeViewNamespacePath.Item.AbsoluteFilePath,
            () => NotifyCopyCompleted(treeViewNamespacePath.Item));

        copyFileMenuOption.OnClick?.Invoke();

        return Task.CompletedTask;
    }

    private Task InvokePasteClipboard(ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is null ||
            activeNode is not TreeViewNamespacePath treeViewNamespacePath ||
            treeViewNamespacePath.Item is null)
        {
            return Task.CompletedTask;
        }

        MenuOptionRecord pasteMenuOptionRecord;

        if (treeViewNamespacePath.Item.AbsoluteFilePath.IsDirectory)
        {
            pasteMenuOptionRecord = _commonMenuOptionsFactory.PasteClipboard(
                treeViewNamespacePath.Item.AbsoluteFilePath,
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
            var parentDirectory = (IAbsoluteFilePath)treeViewNamespacePath
                .Item.AbsoluteFilePath.Directories.Last();

            pasteMenuOptionRecord = _commonMenuOptionsFactory.PasteClipboard(
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
        return Task.CompletedTask;
    }

    private Task InvokeCutFile(ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is null ||
            activeNode is not TreeViewNamespacePath treeViewNamespacePath ||
            treeViewNamespacePath.Item is null)
        {
            return Task.CompletedTask;
        }

        var parent = treeViewNamespacePath.Parent as TreeViewNamespacePath;

        MenuOptionRecord cutFileOptionRecord = _commonMenuOptionsFactory.CutFile(
            treeViewNamespacePath.Item.AbsoluteFilePath,
            () => NotifyCutCompleted(
                treeViewNamespacePath.Item,
                parent));

        cutFileOptionRecord.OnClick?.Invoke();
        return Task.CompletedTask;
    }

    private async Task InvokeOpenInEditorAsync(
        ITreeViewCommandParameter treeViewCommandParameter,
        bool shouldSetFocusToEditor)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is null ||
            activeNode is not TreeViewNamespacePath treeViewNamespacePath ||
            treeViewNamespacePath.Item is null)
        {
            return;
        }

        await EditorState.OpenInEditorAsync(
            treeViewNamespacePath.Item.AbsoluteFilePath,
            shouldSetFocusToEditor,
            _dispatcher,
            _textEditorService,
            _blazorStudioComponentRenderers,
            _fileSystemProvider,
            _backgroundTaskQueue);
    }

    private async Task ReloadTreeViewModel(
        TreeViewNoType? treeViewModel)
    {
        if (treeViewModel is null)
            return;

        await treeViewModel.LoadChildrenAsync();

        _treeViewService.ReRenderNode(
            SolutionExplorerDisplay.TreeViewSolutionExplorerStateKey,
            treeViewModel);

        _treeViewService.MoveUp(
            SolutionExplorerDisplay.TreeViewSolutionExplorerStateKey,
            false);
    }
}