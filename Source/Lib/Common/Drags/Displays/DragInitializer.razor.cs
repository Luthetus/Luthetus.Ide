using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Drags.Displays;

public partial class DragInitializer : FluxorComponent
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private string StyleCss => DragStateWrap.Value.ShouldDisplay
        ? string.Empty
        : "display: none;";

    /// <summary>
    /// Preferably the throttling logic here would be moved out of the drag initializer itself so one can choose to add it themselves, or take the full stream.
    /// </summary>
    private ThrottleAsync _throttleDispatchSetDragStateActionOnMouseMove = new(TimeSpan.FromMilliseconds(5_000));

	private IDropzone? _onMouseOverDropzone = null;

    private DragState.WithAction ConstructClearDragStateAction()
    {
		_onMouseOverDropzone = null;

        return new DragState.WithAction(inState => inState with
        {
            ShouldDisplay = false,
            MouseEventArgs = null,
			Drag = null,
        });
    }

    private async Task DispatchSetDragStateActionOnMouseMoveAsync(MouseEventArgs mouseEventArgs)
    {
        /*
         * This scenario is a perfect example of the issue I'm facing (2024-05-10)
         * Goal: When creating a throttle type: how does one provide a Func<Task>
         *       that will immediately be invoked if the throttle delay is completed.
         * ==========================================================================
         * 
         * I need to continually rewrite my goal until the wording makes sense.
         * ==========================================================================
         * 
         * Goal:
         *     -Create Throttle instance
         *     -Invoke 'PushEvent' with parameter Func<Task>
         *     -if (firstInvocation)
         *          -await Func<Task>()
         *     -else
         *          -_ = Task.Run(async () => await Func<Task>())
         * 
         * Describing the goal in this manner is incomplete in regards to the overall goal,
         * I probably need to implement the throttle piece by piece.
         * 
         * I want to have the throttle await not just a delay, but to also await the previous
         * task prior to starting the new one, even if the delay is complete.
         * 
         * So, what is the simplest throttle I can write? I need to start there.
         * ==========================================================================
         * 
         * The simplest throttle is to:
         *     -Accept as a method parameter a Func<Task>
         *     -if (delayTask.IsComplete)
         *          -await Func<Task>()
         *     -else
         *          -_ = Task.Run(async () => await Func<Task>());
         *      -finally
         *           -delayTask = Task.Run(async () => await Task.Delay(delayTimeSpan));
         * 
         * The questions that I have about the simplest throttle need to be answered
         * prior to making a more complex one.
         * 
         * I have an issue: 
         *     -Should the invoker of the Throttle instance await the result?
         *     -Or, should the Throttle instance always be "fire and forget"?
         * Why is this an issue?:
         *     -If I create a throttle for 'OnMouseMove' on the dialog being moved around,
         *          at a throttle delay of 5 seconds, the UI is frozen for those
         *          5 seconds.
         *     -I do not fully understand what '.ConfigureAwait(false)' does.
         *          -This begs the question, "if I added '.ConfigureAwait(false)'
         *               to the 5 secon throttle problem, would the UI no longer freeze?"
         *     -Additionally, there are a limited amount of ways I can write this code,
         *          I could write every implementation as their own component and compare them.
         *     -The major pain I'm having, is that I don't understand how to unit test the
         *          performance differences between WASM and ServerSide, Blazor.
         *          Everything seems to run fine in ServerSide, then I have to track
         *          down where I'm blocking the "UI thread" for WASM to run well.
         *          -What I mean to say is, how can I test this instead of manually
         *               going through the WASM version and seeing if the "UI thread" ends
         *               up getting blocked.
         * Another issue:
         *     -If the Throttle instance is always "fire and forget"?
         *          -Is there any overhead in constantly creating a "fire and forget" task?
         *          -How does one deterministically execute the "fire and forget" task?
         *               -I want the invoker of the throttle to await their task once
         *                    the delay is over.
         *                    -In order to do this I'd need the invoker to await the delay,
         *                         and this isn't a good solution as was described with the
         *                         5 second throttle delay earlier.
         *                         -So, I end up having the delay be "fire and forget",
         *                              in addition the task be "fire and forget".
         *                              -After the delay is up, it does not seem guaranteed
         *                                   that the task will then immediately be executed.
         *     -Run the throttle on a BackgroundService?
         *          -It seems I can rely on a 'BackgroundService' to immediately respond
         *               when I ask it to execute a Task. (then again I'm not even sure if this is true).
         *          -This solution seems excessive though.
         *     -Perhaps a 'Producer-Consumer' like implementation, maybe with a BlockingCollection, would work?
         *          -But again, who is going to await the BlockingCollection?
         *               -If I use a Task.Run(async () => await BlockingCollection...()), I have no idea
         *                    when this will be executed, right?
         */

        await _throttleDispatchSetDragStateActionOnMouseMove.PushEvent(_ =>
        {
            if ((mouseEventArgs.Buttons & 1) != 1)
            {
                Dispatcher.Dispatch(ConstructClearDragStateAction());
            }
            else
            {
                Dispatcher.Dispatch(new DragState.WithAction(inState => inState with
                {
                    ShouldDisplay = true,
                    MouseEventArgs = mouseEventArgs,
                }));
            }

            return Task.CompletedTask;
        }).ConfigureAwait(false);
    }

    private async Task DispatchSetDragStateActionOnMouseUp(MouseEventArgs mouseEventArgs)
    {
		var dragState = DragStateWrap.Value;
		var localOnMouseOverDropzone = _onMouseOverDropzone;

        await _throttleDispatchSetDragStateActionOnMouseMove.PushEvent(async _ =>
        {
            Dispatcher.Dispatch(ConstructClearDragStateAction());

			var draggableViewModel = dragState.Drag;
			if (draggableViewModel is not null)
				await draggableViewModel.OnDragEndAsync(mouseEventArgs, localOnMouseOverDropzone);
        }).ConfigureAwait(false);
    }

	private string GetIsActiveCssClass(IDropzone dropzone)
	{
		var onMouseOverDropzoneKey = _onMouseOverDropzone?.DropzoneKey ?? Key<IDropzone>.Empty;

		return onMouseOverDropzoneKey == dropzone.DropzoneKey
            ? "luth_active"
			: string.Empty;
	}
}