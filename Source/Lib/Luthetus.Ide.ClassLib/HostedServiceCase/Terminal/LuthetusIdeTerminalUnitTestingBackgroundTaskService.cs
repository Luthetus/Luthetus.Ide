using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.HostedServiceCase.Terminal;

public class LuthetusIdeTerminalUnitTestingBackgroundTaskService : ILuthetusIdeTerminalBackgroundTaskService
{
    public IBackgroundTask? ExecutingBackgroundTask => throw new NotImplementedException();
    public ImmutableArray<IBackgroundTask> PendingBackgroundTasks => throw new NotImplementedException();
    public ImmutableArray<IBackgroundTask> CompletedBackgroundTasks => throw new NotImplementedException();

    public event Action? ExecutingBackgroundTaskChanged;

    public void QueueBackgroundWorkItem(
        IBackgroundTask backgroundTask)
    {
        backgroundTask
            .InvokeWorkItem(CancellationToken.None)
            .Wait();
    }

    public Task<IBackgroundTask?> DequeueAsync(
        CancellationToken cancellationToken)
    {
        return Task.FromResult(default(IBackgroundTask?));
    }

    public void SetExecutingBackgroundTask(IBackgroundTask? backgroundTask)
    {
        throw new NotImplementedException();
    }
}
