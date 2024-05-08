namespace Luthetus.Common.RazorLib.Reactives.Models;

public class ThrottleAsync
{
    private readonly SemaphoreSlim _workItemsStackSemaphoreSlim = new(1, 1);
    private readonly Stack<Func<CancellationToken, Task>> _workItemsStack = new();
    private readonly SemaphoreSlim _previousSemaphoreSlim = new(1, 1);

    private CancellationTokenSource _throttleCancellationTokenSource = new();
    private Task _throttleDelayTask = Task.CompletedTask;
    private Task _previousTask = Task.CompletedTask;

    public bool ShouldWaitForPreviousWorkItemToComplete { get; } = true;

    public ThrottleAsync(TimeSpan throttleTimeSpan)
    {
        ThrottleTimeSpan = throttleTimeSpan;
    }

    /// <summary>
    /// The default value for <see cref="ShouldWaitForPreviousWorkItemToComplete"/> is true
    /// </summary>
    public ThrottleAsync(TimeSpan throttleTimeSpan, bool shouldWaitForPreviousWorkItemToComplete)
    {
        ThrottleTimeSpan = throttleTimeSpan;
        ShouldWaitForPreviousWorkItemToComplete = shouldWaitForPreviousWorkItemToComplete;
    }

    public TimeSpan ThrottleTimeSpan { get; }

    public async Task PushEvent(Func<CancellationToken, Task> workItem)
    {
        await _workItemsStackSemaphoreSlim.WaitAsync().ConfigureAwait(false);

        _workItemsStack.Push(workItem);
        if (_workItemsStack.Count > 1)
            return;

        await _previousSemaphoreSlim.WaitAsync().ConfigureAwait(false);
        if (ShouldWaitForPreviousWorkItemToComplete)
        {
            await _previousTask.ConfigureAwait(false);
            _previousSemaphoreSlim.Release();
        }
        await _throttleDelayTask.ConfigureAwait(false);

        var mostRecentWorkItem = _workItemsStack.Pop();
        _workItemsStack.Clear();

        await _previousSemaphoreSlim.WaitAsync().ConfigureAwait(false);
        _workItemsStackSemaphoreSlim.Release();
        _throttleDelayTask = Task.Delay(ThrottleTimeSpan);
        _previousTask = mostRecentWorkItem.Invoke(CancellationToken.None);
        _previousSemaphoreSlim.Release();

        /*
         Question is:
            How do I set the task that was pop'd as a field,
            without doing so from within the _workItemsStackSemaphoreSlim.

        Because, I need to:
            -push 'scrollEvent'
            -block any further pushes
            -this lets me thread-safely pop
            |
            -NOTE: at this step it is where things get tricky
            |
            -I need to capture the reference to the Task which was pop'd
                -While still preventing any pushes
                -The result of this is "could the task start from within
                 the semaphore slim"?
                -I need to capture the reference to the started task,
                -without letting the task have any chance to run until
                     I've released the _workItemsStackSemaphoreSlim.
                -The reason I need to capture the reference to the task prior
                     to releasing the semaphore slim is: this throttle
                     must wait for the previously pop'd event to complete
                     prior to poping the next task.
                -Idea: there needs to a second thread-safe concept that I can use
                           to indicate that there is intent to set the pop'd task.
                -This sounds like another semaphore slim, that wraps the current _workItemsStackSemaphoreSlim.
                     -Because, I cannot enter the _workItemsStackSemaphoreSlim until the previously pop'd
                          task has been captured as a field.
                     -So, maybe I could have a _intentSemaphoreSlim (name subject to change),
                          and immediately prior to releasing the _workItemsStackSemaphoreSlim,
                          I enter the _intentSemaphoreSlim.
                     -Furthermore, wrapping the _workItemsStackSemaphoreSlim would be an await to enter
                          the _intentSemaphoreSlim.
                     -This sounds close to the solution but seems wrong.
                     -During the time where the task is pop'd, but not yet awaiting the _intentSemaphoreSlim,
                          if an invocation to 'PushEvent(...)' is done, it could enter the
                          _intentSemaphoreSlim.
                     -Because the _intentSemaphoreSlim is await-entered prior to relasing the _workItemsStackSemaphoreSlim
                          it should be safe to swap the order of the initial semaphore logic. _workItemsStackSemaphoreSlim
                          needs to be entered prior to attempting to enter _intentSemaphoreSlim.
                     -This insures a previous invocation of 'PushEvent(...)' has time to enter the '_intentSemaphoreSlim'.
                     -At this point the field _previousTask can be set. And the new invocation to 'PushEvent(...)'
                          can enter the _intentSemaphoreSlim semaphore, then await the _previousTask,
                          and afterwards release the _intentSemaphoreSlim semaphore.
                     -This feels like the answer.
         */
    }

    public void Dispose()
    {
        _throttleCancellationTokenSource.Cancel();
    }
}