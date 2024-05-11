using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Reactives.Models.Internals.Async;
using Luthetus.Common.RazorLib.Reactives.Models.Internals;
using Luthetus.Common.RazorLib.Reactives.Models.Internals.Synchronous;
using Luthetus.Common.RazorLib.Reactives.Models;

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

    public static ThrottleWip ThrottleWip = new ThrottleWip(TimeSpan.FromMilliseconds(100));

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
        var workItem = new Func<Task>(() =>
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
        });

        if (ThrottleWip is ICounterThrottleAsync throttleAsync)
        {
            Func<double, Task>? progressFunc;

            if (ThrottleWip.ThrottleTimeSpan.TotalMilliseconds >= 1_000)
            {
                progressFunc = async d =>
                 {
                     var HACK_ReRenderProgress = throttleAsync.HACK_ReRenderProgress;

                     if (HACK_ReRenderProgress is not null)
                         await HACK_ReRenderProgress.Invoke(d);
                 };
            }
            else
            {
                progressFunc = null;
            }

            await throttleAsync.PushEvent(
                workItem,
                progressFunc);
        }
        else if (ThrottleWip is ICounterThrottleSynchronous throttleSynchronous)
        {
            Func<double, Task>? progressFunc;

            if (ThrottleWip.ThrottleTimeSpan.TotalMilliseconds >= 1_000)
            {
                progressFunc = async d =>
                {
                    var HACK_ReRenderProgress = throttleSynchronous.HACK_ReRenderProgress;

                    if (HACK_ReRenderProgress is not null)
                        await HACK_ReRenderProgress.Invoke(d);
                };
            }
            else
            {
                progressFunc = null;
            }

            throttleSynchronous.PushEvent(
                workItem,
                progressFunc);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private async Task DispatchSetDragStateActionOnMouseUp(MouseEventArgs mouseEventArgs)
    {
		var dragState = DragStateWrap.Value;
		var localOnMouseOverDropzone = _onMouseOverDropzone;

        var workItem = new Func<Task>(async () =>
        {
            Dispatcher.Dispatch(ConstructClearDragStateAction());

            var draggableViewModel = dragState.Drag;
            if (draggableViewModel is not null)
                await draggableViewModel.OnDragEndAsync(mouseEventArgs, localOnMouseOverDropzone);
        });

        if (ThrottleWip is ICounterThrottleAsync throttleAsync)
        {
            Func<double, Task>? progressFunc;

            if (ThrottleWip.ThrottleTimeSpan.TotalMilliseconds >= 1_000)
            {
                progressFunc = async d =>
                {
                    var HACK_ReRenderProgress = throttleAsync.HACK_ReRenderProgress;

                    if (HACK_ReRenderProgress is not null)
                        await HACK_ReRenderProgress.Invoke(d);
                };
            }
            else
            {
                progressFunc = null;
            }

            await throttleAsync.PushEvent(
                workItem,
                progressFunc).ConfigureAwait(false);
        }
        else if (ThrottleWip is ICounterThrottleSynchronous throttleSynchronous)
        {
            Func<double, Task>? progressFunc;

            if (ThrottleWip.ThrottleTimeSpan.TotalMilliseconds >= 1_000)
            {
                progressFunc = async d =>
                {
                    var HACK_ReRenderProgress = throttleSynchronous.HACK_ReRenderProgress;

                    if (HACK_ReRenderProgress is not null)
                        await HACK_ReRenderProgress.Invoke(d);
                };
            }
            else
            {
                progressFunc = null;
            }

            throttleSynchronous.PushEvent(
                workItem,
                progressFunc);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

	private string GetIsActiveCssClass(IDropzone dropzone)
	{
		var onMouseOverDropzoneKey = _onMouseOverDropzone?.DropzoneKey ?? Key<IDropzone>.Empty;

		return onMouseOverDropzoneKey == dropzone.DropzoneKey
            ? "luth_active"
			: string.Empty;
	}

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
         * ==========================================================================
         * 
         * I just ran the 5 second throttle on ServerSide and it seems to invoke the task,
         * with no regard for the 5 second throttle at all.
         * 
         * I think I'm my own worst enemy here. I keep writing the throttle to work for
         * either WASM or ServerSide, then I swap to the other, it doesn't work,
         * so I re-write, then swap, it doesn't work, re-write, etc.....
         * 
         * I need to just understand how threads work.
         * 
         * I'm wondering, I currently have '.ConfigureAwait(false);'.
         * So, is it the case that WASM is single threaded,
         * therefore '.ConfigureAwait(false);' has no impact in this situation,
         * with or without its stuck for 5 seconds on the delay.
         * 
         * Where as:
         * If ServerSide is multi-threaded '.ConfigureAwait(false);', tells the
         * thread that the 'PushEvent' can be ran on a different 'thread/context'.
         * This allows the 5 second delay to be awaited on a thread different
         * than the UI thread.
         * 
         * If I remove the '.ConfigureAwait(false);' is ServerSide going to
         * await the 5 second delay with the UI thread when running ServerSide?
         * 
         * Further confusion comes from single threaded code having a concurrency
         * model that permits the illusion of code running in parallel.
         * But I should do like I said, remove that configure await and see what happens.
         * 
         * Okay, on WASM. I ran with '.ConfigureAwait(false);', and without,
         * at a 5 second delay. In both situations the UI was locked
         * for those 5 seconds of delay.
         * 
         * Now for ServerSide...
         *     -With '.ConfigureAwait(false);'
         *          the UI got blocked for those 5 seconds of delay.
         *     -Without the '.ConfigureAwait(false);'
         *          the UI got blocked for those 5 seconds of delay.
         *          
         * I'm thoroughly confused now because I just ran ServerSide
         * with the '.ConfigureAwait(false);' about 5 minutes ago,
         * and the UI wasn't getting blocked for the 5 seconds of delay.
         * 
         * I think I ran it with Release mode during that 5 minutes ago period.
         * I also wonder if I somehow forgot to re-run the ServerSide application,
         * and am running an old version that doesn't have '.ConfigureAwait(false);'?
         * 
         * I re-ran the ServerSide app with the '.ConfigureAwait(false);'
         * to be sure that I was running the correct code.
         * It still locks for the 5 seconds of delay.
         * 
         * I'm going to try Release mode...
         * With Release mode and '.ConfigureAwait(false);' the
         * delay is locking the UI.
         * 
         * I feel as though all I've done is type into this comment the entire time,
         * and that I haven't touched anything, thereby am I crazy
         * for having this memory of how 5+ minutes ago ServerSide with '.ConfigureAwait(false);'
         * didn't have the delay locking the UI???
         * 
         * I checked git and beyond '.ConfigureAwait(false);' I do see that I had changed
         * the method itself from being 'Task' to 'async Task' in order to await
         * the configured Task.
         * 
         * From my understanding, if I use the 'async' keyword, then a state machine 
         * is made for the method invocation.
         * 
         * I don't use the 'async' keyword, then someone upstream is expected to await
         * my Task, and at that point the state machine is made.
         * 
         * Therefore, its beneficial for me to simply return a Task, as opposed to awaiting it,
         * where possible. (to avoid the state machine overhead).
         * 
         * The reason I added the 'async' keyword, was because I don't think I can return
         * a ConfiguredTaskAwaitable when the method signature says it will return a Task.
         * 
         * For this reason I had to be the one to await the ConfiguredTaskAwaitable.
         * And turning the method to async, "implicitly-returns a task" so to speak.
         * 
         * I have no proof that I cannot return a ConfiguredTaskAwaitable,
         * so I should try it now.
         * 
         * Trying to return 'ConfiguredTaskAwaitable' from a method with the return signature
         * 'Task' gives an error that 'ConfiguredTaskAwaitable' is not implicitly convertable
         * to 'Task'.
         * 
         * ==================================================================================
         * 
         * Well, it seems there are 3 cases foreach hosting model:
         *     -Return the Task (no async keyword)
         *     -Await the Task (with async keyword)
         *     -Await the ConfiguredTaskAwaitable (with async keyword)
         *     
         * I guess I want a checklist so I'm going to type out all 6 cases:
         *     [ ] WASM
         *          [ ] Return the Task (no async keyword)
         *          [ ] Await the Task (with async keyword)
         *          [ ] Await the ConfiguredTaskAwaitable (with async keyword)
         *     [ ] ServerSide
         *          [ ] Return the Task (no async keyword)
         *          [ ] Await the Task (with async keyword)
         *          [ ] Await the ConfiguredTaskAwaitable (with async keyword)
         *          
         * Beyond this though, I've written the Throttle logic
         * quite a few times, and these various times were done differently.
         * 
         * I should keep handy the various ways I've written the throttle.
         * Then I can add an even further top-hierarchy to my checklist,
         * which would be the implementation I went for with the throttle.
         * 
         * ==================================================================================
         * 
         * A side thought in my head: if WASM is single threaded, and by way of a
         * concurrency model (async-await) one can create the illusion of parallelism
         * with a single thread, why did I earlier today see no changes when using
         * '.ConfigureAwait(false)' with the WASM host, vs not using it?
         * Presumably, the "single threaded illusion" here would be the task for the delay
         * starting, then the UI thread takes control of the thread until its somehow
         * notified that the delay if over?
         * 
         * I need to pay attention and see if I can find any similarities/differences between
         * using '.ConfigureAwait(false)' when running with a single threaded application vs multi threaded.
         * (further side note: is it single threaded runtime or single threaded application?)
         */

    /*
     * This scenario is a perfect example of the issue I'm facing (2024-05-10)
     * Goal: When creating a throttle type: how does one provide a Func<Task>
     *       that will immediately be invoked if the throttle delay is completed.
     * ==========================================================================
     * 
     * The dialog misses the final mouse move event, BUT there is a reason for this.
     * It is due to the onmouseup event overwritting the final mouse move event
     * since the same throttle is used for both.
     * ==========================================================================
     * 
     * To avoid confusion: try interacting with the input elements that
     * are on the dialog, as opposed to the website behind the dialog.
     * When the drag events are happening there is an invisible div
     * that covers the screen with a lower z-index than the dialogs,
     * so those will never be interactable with, and is unrelated.
     * ==========================================================================
     * 
     * -Render the CounterThrottleAsyncDisplay in a dialog
     * -Use the same CounterThrottleAsync instance that is being used in the CounterThrottleAsyncDisplay
     *      but for the dialog drag events.
     * -Cascade any state has changed events from the dialog to the CounterThrottleAsyncDisplay
     */

    // NOTE: Does it make a difference if my Task runs synchronously or not?
    //
    // NOTE: The Dispatcher will cause re-renders, is this Dispatch causing me
    //       to hijack the UI thread?
    //
    // (2024-05-11) I figured out the problem...
    //              I'm so tired, and feel so stupid;
    //              I don't know whether if should cry or laugh...
    //              The drag initializer renders an invisible div above everything else,
    //              while a drag event is occurring, in order to capture the onmousemove
    //              events.
    //
    //              I couldn't interact with the UI, not because the app was frozen,
    //              but because it was covered by this component's invisible div.
    //
    //              Furthermore, by happenstance, the onmouseup event is what
    //              makes the div no longer render.
    //              Of which, this event is on the same throttle as the onmousemove.
    //              Therefore, the div only went away after the final throttle event,
    //              which took at least the time of the throttle delay (2 seconds).
    //
    //              aaaaaaaaaaaaaaaaaaaaaaaaaaaaaahhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh
    //
    //              I added 'background-color: orange;' to the '.luth_drag-initializer'
    //              css selector.
    //             
    //              The entire site is just one big orange div once I start dragging.
    //
    //              I've decided I'm going to go cry...
    //
    //              ...some other day because THIS IS THE MOMENT WE'VE ALL BEEN WATING FOR
    //              LET'S GOooooOOOoOOOOOOooo.
    //
    //              And then everyone clapped.
}