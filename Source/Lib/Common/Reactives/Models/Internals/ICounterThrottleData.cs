namespace Luthetus.Common.RazorLib.Reactives.Models.Internals;

public interface ICounterThrottleData
{
    public object IdLock { get; }
    public object ExecutedCountLock { get; }
    public Stack<Func<Task>> WorkItemStack { get; }
    public Task DelayTask { get; }
    public Task WorkItemTask { get; }
    public TimeSpan ThrottleTimeSpan { get; }
    public SynchronizationContext? PushEventStart_SynchronizationContext { get; }
    public SynchronizationContext? PushEventEnd_SynchronizationContext { get; }
    public Thread? PushEventStart_Thread { get; }
    public Thread? PushEventEnd_Thread { get; }
    public (int Id, DateTime DateTime) PushEventStart_DateTimeTuple { get; }
    public (int Id, DateTime DateTime) PushEventEnd_DateTimeTuple { get; }
    public int WorkItemsExecutedCount { get; }
    public int GetId { get; }
}

public static class ThrottleProgressHelper
{
    /// <summary>
    /// Parameter is the percent complete in a 0...1 form,
    /// multiply this parameter by the width of the progress bar,
    /// render an element inside the parent with a different background color etc...
    /// </summary>
    /// <param name="progressFunc">Parameter is the percent complete in a 0...1 form</param>
    public static async Task DelayWithProgress(Func<double, Task> progressFunc, TimeSpan timeSpan)
    {
        // Every x miliseconds post update.
        var updateFrequencyMilliseconds = 250.0;
        var divisions = timeSpan.TotalMilliseconds / updateFrequencyMilliseconds;
        divisions = Math.Floor(divisions);

        await progressFunc.Invoke(0);

        if (divisions > 0)
        {
            for (int i = 0; i < divisions; i++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(updateFrequencyMilliseconds));
                await progressFunc.Invoke(i / divisions);
            }

            // Await the remaining time
            await Task.Delay(TimeSpan.FromMilliseconds(timeSpan.TotalMilliseconds % divisions));
        }
        else
        {
            await Task.Delay(timeSpan);
        }

        await progressFunc.Invoke(1);
    }
}