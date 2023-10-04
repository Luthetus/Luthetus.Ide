using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;

namespace Luthetus.Common.RazorLib.TreeViews.Models;

/// <summary>
/// To implement custom KeyboardEvent handling logic one should
/// inherit <see cref="TreeViewKeyboardEventHandler"/> and override the corresponding method.
/// </summary>
public class TreeViewKeyboardEventHandler
{
    protected readonly ITreeViewService TreeViewService;

    public TreeViewKeyboardEventHandler(ITreeViewService treeViewService)
    {
        TreeViewService = treeViewService;
    }

    /// <summary>Used for handling "onkeydownwithpreventscroll" events within the user interface</summary>
    public virtual void OnKeyDown(TreeViewCommandParameter commandParameter)
    {
        if (commandParameter.KeyboardEventArgs is null)
            return;

        switch (commandParameter.KeyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
                TreeViewService.MoveLeft(
                    commandParameter.TreeViewState.Key,
                    commandParameter.KeyboardEventArgs.ShiftKey);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
                TreeViewService.MoveDown(
                    commandParameter.TreeViewState.Key,
                    commandParameter.KeyboardEventArgs.ShiftKey);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
                TreeViewService.MoveUp(
                    commandParameter.TreeViewState.Key,
                    commandParameter.KeyboardEventArgs.ShiftKey);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
                TreeViewService.MoveRight(
                    commandParameter.TreeViewState.Key,
                    commandParameter.KeyboardEventArgs.ShiftKey);
                break;
            case KeyboardKeyFacts.MovementKeys.HOME:
                TreeViewService.MoveHome(
                    commandParameter.TreeViewState.Key,
                    commandParameter.KeyboardEventArgs.ShiftKey);
                break;
            case KeyboardKeyFacts.MovementKeys.END:
                TreeViewService.MoveEnd(
                    commandParameter.TreeViewState.Key,
                    commandParameter.KeyboardEventArgs.ShiftKey);
                break;
            default:
                break;
        }

        _ = Task.Run(async () => await OnKeyDownAsync(commandParameter));
    }

    /// <summary>Used for handling "onkeydownwithpreventscroll" events within the user interface</summary>
    public virtual Task OnKeyDownAsync(TreeViewCommandParameter commandParameter)
    {
        return Task.CompletedTask;
    }
}