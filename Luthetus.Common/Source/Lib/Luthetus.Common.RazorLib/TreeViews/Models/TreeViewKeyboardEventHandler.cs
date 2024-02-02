using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.TreeViews.Models;

/// <summary>
/// To implement custom KeyboardEvent handling logic one should
/// inherit <see cref="TreeViewKeyboardEventHandler"/> and override the corresponding method.
/// </summary>
public class TreeViewKeyboardEventHandler
{
    protected readonly ITreeViewService TreeViewService;
    protected readonly IBackgroundTaskService BackgroundTaskService;

    public TreeViewKeyboardEventHandler(
		ITreeViewService treeViewService,
		IBackgroundTaskService backgroundTaskService)
    {
        TreeViewService = treeViewService;
		BackgroundTaskService = backgroundTaskService;
    }

    /// <summary>Used for handling "onkeydownwithpreventscroll" events within the user interface</summary>
    public virtual void OnKeyDown(TreeViewCommandArgs commandArgs)
    {
        if (commandArgs.KeyboardEventArgs is null)
            return;

        switch (commandArgs.KeyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
                TreeViewService.MoveLeft(
                    commandArgs.TreeViewContainer.Key,
                    commandArgs.KeyboardEventArgs.ShiftKey,
					false);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
                TreeViewService.MoveDown(
                    commandArgs.TreeViewContainer.Key,
                    commandArgs.KeyboardEventArgs.ShiftKey,
					false);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
                TreeViewService.MoveUp(
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
                TreeViewService.MoveHome(
                    commandArgs.TreeViewContainer.Key,
                    commandArgs.KeyboardEventArgs.ShiftKey,
					false);
                break;
            case KeyboardKeyFacts.MovementKeys.END:
                TreeViewService.MoveEnd(
                    commandArgs.TreeViewContainer.Key,
                    commandArgs.KeyboardEventArgs.ShiftKey,
					false);
                break;
            default:
                break;
        }

		BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
        	"TreeView.OnKeyDown",
			async () => await OnKeyDownAsync(commandArgs).ConfigureAwait(false));
    }

    /// <summary>Used for handling "onkeydownwithpreventscroll" events within the user interface</summary>
    public virtual Task OnKeyDownAsync(TreeViewCommandArgs commandArgs)
    {
        return Task.CompletedTask;
    }
}