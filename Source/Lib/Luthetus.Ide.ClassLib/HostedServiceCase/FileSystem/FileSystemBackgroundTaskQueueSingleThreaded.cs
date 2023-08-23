using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Ide.ClassLib.HostedServiceCase.FileSystem;

namespace Luthetus.Ide.ClassLib.HostedServiceCase.Terminal;

public class FileSystemBackgroundTaskQueueSingleThreaded : IFileSystemBackgroundTaskQueue
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