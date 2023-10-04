using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

namespace Luthetus.Ide.RazorLib.CompilerServices.Models;

public class CompilerServiceExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly EditorSync _editorSync;

    public CompilerServiceExplorerTreeViewKeyboardEventHandler(
        EditorSync editorSync,
        ITreeViewService treeViewService)
        : base(treeViewService)
    {
        _editorSync = editorSync;
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
            default:
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
}