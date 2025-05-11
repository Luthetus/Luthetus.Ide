using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.TreeViews.Models;

/// <summary>
/// To implement custom KeyboardEvent handling logic one should
/// inherit <see cref="TreeViewKeyboardEventHandler"/> and override the corresponding method.
/// </summary>
public class TreeViewKeyboardEventHandler
{
    protected readonly ITreeViewService TreeViewService;
    protected readonly BackgroundTaskService BackgroundTaskService;

    public TreeViewKeyboardEventHandler(
		ITreeViewService treeViewService,
		BackgroundTaskService backgroundTaskService)
    {
        TreeViewService = treeViewService;
		BackgroundTaskService = backgroundTaskService;
    }

    /// <summary>
    /// Invoked, and awaited, as part of the async UI event handler for 'onkeydownwithpreventscroll' events.<br/><br/>
    /// 
    /// The synchronous version: '<see cref="OnKeyDown(TreeViewCommandArgs)"/>' will be invoked
    /// immediately from within this method, to allow the synchronous code to block the UI purposefully.
    /// 
    /// Any overrides of this method are intended to have 'base.MethodBeingOverridden()' prior to their code.<br/><br/>
    /// </summary>
    public virtual Task OnKeyDownAsync(TreeViewCommandArgs commandArgs)
    {
        // Run the synchronous code first
        OnKeyDown(commandArgs);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Invoked, and awaited, as part of the synchronous UI event handler for 'onkeydownwithpreventscroll' events.<br/><br/>
    /// 
    /// This method is invoked by the async version: '<see cref="OnKeyDownAsync(TreeViewCommandArgs)"/>'.<br/><br/>
    /// 
    /// Any overrides of this method are intended to have 'base.MethodBeingOverridden()' prior to their code.<br/><br/>
    /// </summary>
    protected virtual void OnKeyDown(TreeViewCommandArgs commandArgs)
    {
        if (commandArgs.KeyboardEventArgs is null)
            return;

        switch (commandArgs.KeyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
                TreeViewService.ReduceMoveLeftAction(
                    commandArgs.TreeViewContainer.Key,
                    commandArgs.KeyboardEventArgs.ShiftKey,
					false);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
                TreeViewService.ReduceMoveDownAction(
                    commandArgs.TreeViewContainer.Key,
                    commandArgs.KeyboardEventArgs.ShiftKey,
					false);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
                TreeViewService.ReduceMoveUpAction(
                    commandArgs.TreeViewContainer.Key,
                    commandArgs.KeyboardEventArgs.ShiftKey,
					false);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
                TreeViewService.MoveRight(
                    commandArgs.TreeViewContainer.Key,
                    commandArgs.KeyboardEventArgs.ShiftKey,
					false);
                break;
            case KeyboardKeyFacts.MovementKeys.HOME:
                TreeViewService.ReduceMoveHomeAction(
                    commandArgs.TreeViewContainer.Key,
                    commandArgs.KeyboardEventArgs.ShiftKey,
					false);
                break;
            case KeyboardKeyFacts.MovementKeys.END:
                TreeViewService.ReduceMoveEndAction(
                    commandArgs.TreeViewContainer.Key,
                    commandArgs.KeyboardEventArgs.ShiftKey,
					false);
                break;
            default:
                break;
        }
    }
}