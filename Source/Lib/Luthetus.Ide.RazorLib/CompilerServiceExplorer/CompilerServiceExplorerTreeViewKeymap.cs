using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.Keyboard;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.Menu;
using Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorer;

public class CompilerServiceExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly IMenuOptionsFactory _menuOptionsFactory;
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
    private readonly IDispatcher _dispatcher;
    private readonly ITreeViewService _treeViewService;

    public CompilerServiceExplorerTreeViewKeyboardEventHandler(
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
        {
            CtrlModifiedKeymap(treeViewCommandParameter);
            return;
        }
        else if (treeViewCommandParameter.KeyboardEventArgs.AltKey)
        {
            AltModifiedKeymap(treeViewCommandParameter);
            return;
        }
    }

    private void CtrlModifiedKeymap(ITreeViewCommandParameter treeViewCommandParameter)
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
            default:
                return;
        }
    }

    private void AltModifiedKeymap(ITreeViewCommandParameter treeViewCommandParameter)
    {
        return;
    }

    private void CtrlAltModifiedKeymap(ITreeViewCommandParameter treeViewCommandParameter)
    {
        return;
    }

    private Task NotifyCopyCompleted(NamespacePath namespacePath)
    {
        if (_luthetusCommonComponentRenderers.InformativeNotificationRendererType != null)
        {
            var notificationInformative = new NotificationRecord(
                NotificationKey.NewNotificationKey(),
                "Copy Action",
                _luthetusCommonComponentRenderers.InformativeNotificationRendererType,
                new Dictionary<string, object?>
                {
                {
                    nameof(IInformativeNotificationRendererType.Message),
                    $"Copied: {namespacePath.AbsoluteFilePath.FilenameWithExtension}"
                },
                },
                TimeSpan.FromSeconds(3),
                true,
                null);

            _dispatcher.Dispatch(new NotificationRecordsCollection.RegisterAction(
                notificationInformative));
        }

        return Task.CompletedTask;
    }

    private void InvokeOpenInEditor(
        ITreeViewCommandParameter treeViewCommandParameter,
        bool shouldSetFocusToEditor)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is not TreeViewNamespacePath treeViewNamespacePath)
            return;

        _dispatcher.Dispatch(new EditorState.OpenInEditorAction(
            treeViewNamespacePath.Item.AbsoluteFilePath,
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