namespace Luthetus.Common.RazorLib.Reactives.Models.Internals;

public static class ThrottleProgressHelper
{
    /// <summary>
    /// Parameter is the percent complete in a 0...1 form,
    /// multiply this parameter by the width of the progress bar,
    /// render an element inside the parent with a different background color etc...
    /// </summary>
    /// <param name="progressFunc">Parameter is the percent complete in a 0...1 form</param>
    public static async Task DelayWithProgress(TimeSpan timeSpan, Func<double, Task> progressFunc)
    {
        // Every x miliseconds post update.
        var updateFrequencyMilliseconds = 250.0;
        var divisions = timeSpan.TotalMilliseconds / updateFrequencyMilliseconds;
        divisions = Math.Floor(divisions);

        await progressFunc.Invoke(0).ConfigureAwait(false);

        if (divisions > 0)
        {
            for (int i = 0; i < divisions; i++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(updateFrequencyMilliseconds)).ConfigureAwait(false);
                await progressFunc.Invoke(i / divisions).ConfigureAwait(false);
            }

            // Await the remaining time
            await Task.Delay(TimeSpan.FromMilliseconds(timeSpan.TotalMilliseconds % divisions)).ConfigureAwait(false);
        }
        else
        {
            await Task.Delay(timeSpan).ConfigureAwait(false);
        }

        await progressFunc.Invoke(1).ConfigureAwait(false);
    }
}