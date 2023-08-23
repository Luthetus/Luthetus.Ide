using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;

namespace Luthetus.Ide.ClassLib.HostedServiceCase.Terminal;

public class TerminalBackgroundTaskQueueSingleThreaded : ITerminalBackgroundTaskQueue
{
    public void QueueBackgroundWorkItem(
        IBackgroundTask backgroundTask)
    {
        _ = Task.Run(async () =>
        {
            await backgroundTask
                .InvokeWorkItem(CancellationToken.None);
        });
    }

    public Task<IBackgroundTask?> DequeueAsync(
        CancellationToken cancellationToken)
    {
        return Task.FromResult(default(IBackgroundTask?));
    }
}