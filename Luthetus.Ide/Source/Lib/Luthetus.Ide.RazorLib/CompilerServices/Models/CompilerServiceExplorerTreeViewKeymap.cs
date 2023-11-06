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
            default:
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

    private void InvokeOpenInEditor(TreeViewCommandArgs commandArgs, bool shouldSetFocusToEditor)
    {
        var activeNode = commandArgs.TreeViewContainer.ActiveNode;

        if (activeNode is not TreeViewNamespacePath treeViewNamespacePath)
            return;

        _editorSync.OpenInEditor(treeViewNamespacePath.Item.AbsolutePath, shouldSetFocusToEditor);
        return;
    }
}