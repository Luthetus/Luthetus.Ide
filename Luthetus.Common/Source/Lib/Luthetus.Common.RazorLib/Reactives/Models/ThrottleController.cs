using Luthetus.Common.RazorLib.Reactives.Models;
using System.Collections.Concurrent;
using System.Threading;

namespace Luthetus.Common.Tests.Basis.Reactives.Models;

/// <summary>
/// This class is being made because <see cref="Throttle"/> is not sufficient for what this will do.<br/>
/// 
/// For example, one might use <see cref="Throttle"/> on UI events.<br/>
/// 
/// Perhaps one has a 'throttleOnKeyDown', and a 'throttleOnMouseMove'.<br/>
/// 
/// To most easily illustrate the point, we can give throttleOnKeyDown a throttle delay of '50 miliseconds'.<br/>
/// 
/// Then we can give 'throttleOnMouseMove' a throttle delay of '10 seconds'.<br/>
/// 
/// Now, the user interacts with the UI by selecting a block of text. This would fire the 'throttleOnMouseMove'
/// initially, then start a delay of 10 seconds until the next invocation can occur.<br/>
/// 
/// It is very likely, that the user caused multiple 'onmousemove' UI events to occur.<br/>
/// 
/// So, the user set the anchor for their text selection, then moved the mouse to where they
/// want the selection to end.<br/>
/// 
/// But, because there is a 10 second throttle, only the anchor of their selection had been placed.<br/>
/// 
/// They now go on to, within 10 seconds, presume that their selection was made. So they hit the
/// 'backspace' key, as to delete that selected text.<br/>
/// 
/// But, the throttle delay of the 'throttleOnMouseMove' caused a situation where,
/// the user doesn't have any selection yet.<br/>
/// 
/// So, the 'backspace' ends up deleting the single character at which the first 'onmousemove' UI event
/// put them.<br/>
/// 
/// The goal of this class is to solve, this problem.<br/>
/// 
/// If instead of having 'throttleOnKeyDown', and 'throttleOnMouseMove', one had 'throttleUiEvents'
/// then this issue could be remedied, provided the implementation details of <see cref="Throttle"/>
/// changed a bit.<br/>
/// 
/// If we were to use the <see cref="Throttle"/> to create the 'throttleUiEvents',
/// it wouldn't work.<br/>
/// 
/// This is because the <see cref="Throttle"/> works by collecting events,
/// then firing the most recent one when the throttleDelayTask is completed.
/// At the time of firing the most recent one however, all other events are discarded.<br/>
/// 
/// The discarding of all other events is part of the issue.<br/>
/// 
/// If we make 'throttleUiEvents' have a throttle delay of '10 seconds'.<br/>
/// 
/// And then, a user interacts with the UI and tries to select a block of text.
/// The events for their selecting of text wont be fully handled, if they proceed
/// to type within the 10 seconds of the throttle delay.<br/>
/// 
/// We can assign to each 'throttleUiEvents.FireAsync(async () => {})'
/// an 'id' which can be a string, or anything that can be used to uniquely identify.<br/>
/// 
/// So, the mouse events when the user selects text would now be:
/// 'throttleUiEvents.FireAsync("onmousemove", async () => {})'<br/>
/// 
/// Following that is the onkeydown event. So that would be:
/// 'throttleUiEvents.FireAsync("onkeydown", async () => {})'<br/>
/// 
/// If we ignore the 'Stack' based implementation,
/// one could see that any 'workItems' with the same id,
/// could then be discarded, upon invoking one of those work items.<br/>
/// 
/// And we can keep the 'workItems' which don't share the same id.<br/>
/// 
/// The idea for implementing this logic is to use a Queue instead of a Stack.<br/>
/// 
/// One can then view the previously described text selection scenario as:<br/>
/// 
/// Queue<br/>
/// {<br/>
///     First event -> 0: [ Id: "onmousemove", workItem: async () => {}, consecutiveEntryFunc: async (a, b) => {} ]<br/>
///                    1: [ Id: "onmousemove", workItem: async () => {}, consecutiveEntryFunc: async (a, b) => {} ]<br/>
///                    2: [ Id: "onmousemove", workItem: async () => {}, consecutiveEntryFunc: async (a, b) => {} ]<br/>
///                    3: [ Id: "onmousemove", workItem: async () => {}, consecutiveEntryFunc: async (a, b) => {} ]<br/>
///     Last  event -> 4: [ Id: "onkeydown",   workItem: async () => {}, consecutiveEntryFunc: async (a, b) => {} ]<br/>
/// }<br/>
/// 
/// The implementation of <see cref="Throttle"/> will take consecutive events and only handle the
/// most recent one.<br/>
/// 
/// A similar functionality can be done by using consecutive entries in the Queue, which have the same Id,
/// are merged into a single workItem.<br/>
/// 
/// The details of merging workItems is something this class won't be capable of doing. It couldn't possible know how handle that.
/// But, if the throttle took as a parameter, a Func that took as input 'recentWorkItem' and 'oldWorkItem'.
/// And then output the 'mergedWorkItem'.<br/>
/// 
/// One could merge consecutive workItems that share the same Id, by letter the provider of the workItem
/// decide how to handle it.<br/>
/// 
/// How would data be shared between the consecutive workItems?<br/>
/// 
/// If I have two "onkeydown" workItems that appear consecutively, then the merge might be:<br/>
/// If both workItems are inserting a character, then instead perform an InsertRange and provide both characters together.<br/>
/// 
/// How does the 'recentWorkItem' and 'oldWorkItem' expose the character they are going to insert, during the
/// consecutiveEntryFunc?<br/>
/// 
/// My thought, is to change from how <see cref="Throttle"/> is acceping a Func&lt;CancellationToken, Task&gt;
/// to have this class accept a 'ThrottleEvent&lt;T&gt;'.<br/>
/// 
/// This then allows one to attach data to the event by way of the generic type.
/// </summary>
public class ThrottleController
{
    private readonly object _lockSemaphoreSlim = new();
    private readonly ConcurrentQueue<IThrottleEvent> _throttleEventConcurrentQueue = new();

    private CancellationTokenSource _throttleCancellationTokenSource = new();
    private Task _throttleDelayTask = Task.CompletedTask;
    private Task _previousWorkItemTask = Task.CompletedTask;
    private SemaphoreSlim _semaphoreSlim = new(1, 1);

    public void FireAndForget(IThrottleEvent throttleEvent)
    {
        _throttleEventConcurrentQueue.Enqueue(throttleEvent);

        if (_throttleEventConcurrentQueue.Count > 1)
            return;

        _ = Task.Run(DequeueAsync).ConfigureAwait(false);
    }

    private async Task DequeueAsync()
    {
        try
        {
            lock (_lockSemaphoreSlim)
            {
                if (_semaphoreSlim.CurrentCount <= 0)
                    return;
            }

            await _semaphoreSlim.WaitAsync();

            while (true)
            {
                await _throttleDelayTask.ConfigureAwait(false);
                await _previousWorkItemTask.ConfigureAwait(false);

                CancellationToken cancellationToken;

                if (_throttleEventConcurrentQueue.TryDequeue(out var oldEvent) && oldEvent is not null)
                {
                    while (oldEvent.ConsecutiveEntryFunc is not null &&
                            _throttleEventConcurrentQueue.TryPeek(out var recentEvent) && recentEvent is not null &&
                                oldEvent.Id == recentEvent.Id)
                    {
                        var consecutiveResult = oldEvent.ConsecutiveEntryFunc.Invoke((oldEvent, recentEvent));
                        
                        if (consecutiveResult is not null)
                        {
                            // Because the 'ConsecutiveEntryFunc' function successfully merged
                            // the two work items, then dequeue the recentEvent since it will be handled.
                            _throttleEventConcurrentQueue.TryDequeue(out recentEvent);
                            oldEvent = consecutiveResult;
                        }
                    }

                    _throttleCancellationTokenSource.Cancel();
                    _throttleCancellationTokenSource = new();

                    cancellationToken = _throttleCancellationTokenSource.Token;

                    _throttleDelayTask = Task.Run(async () =>
                    {
                        await Task.Delay(oldEvent.ThrottleTimeSpan).ConfigureAwait(false);
                    }, CancellationToken.None);

                    _previousWorkItemTask = Task.Run(async () =>
                    {
                        await oldEvent.WorkItem.Invoke(oldEvent, CancellationToken.None);
                    }, CancellationToken.None);
                }
                else
                {
                    break;
                }
            }
        }
        finally
        {
            lock (_lockSemaphoreSlim)
            {
                _semaphoreSlim.Release();
            }
        }
    }

    public void Dispose()
    {
        _throttleCancellationTokenSource.Cancel();
    }
}
