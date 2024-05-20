using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.FileSystems.Models;

public class LuthetusIdeFileSystemBackgroundTaskApi
{
    private readonly LuthetusIdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDispatcher _dispatcher;

    public LuthetusIdeFileSystemBackgroundTaskApi(
        LuthetusIdeBackgroundTaskApi ideBackgroundTaskApi,
        IFileSystemProvider fileSystemProvider,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _fileSystemProvider = fileSystemProvider;
        _commonComponentRenderers = commonComponentRenderers;
        _backgroundTaskService = backgroundTaskService;
        _dispatcher = dispatcher;
    }

    public Task SaveFile(
        IAbsolutePath absolutePath,
        string content,
        Func<DateTime?, Task> onAfterSaveCompletedWrittenDateTimeFunc,
        CancellationToken cancellationToken = default)
    {
        return _backgroundTaskService.EnqueueAsync(
            Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Save File",
            async () => await SaveFileAsync(
                    absolutePath,
                    content,
                    onAfterSaveCompletedWrittenDateTimeFunc,
                    cancellationToken)
                .ConfigureAwait(false));
    }

    private async Task SaveFileAsync(
        IAbsolutePath absolutePath,
        string content,
        Func<DateTime?, Task> onAfterSaveCompletedWrittenDateTimeFunc,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        var absolutePathString = absolutePath.Value;

        if (absolutePathString is not null &&
            await _fileSystemProvider.File.ExistsAsync(absolutePathString).ConfigureAwait(false))
        {
            await _fileSystemProvider.File.WriteAllTextAsync(absolutePathString, content).ConfigureAwait(false);
        }
        else
        {
            // TODO: Save As to make new file
            NotificationHelper.DispatchInformative("Save Action", "File not found. TODO: Save As", _commonComponentRenderers, _dispatcher, TimeSpan.FromSeconds(7));
        }

        DateTime? fileLastWriteTime = null;

        if (absolutePathString is not null)
        {
            fileLastWriteTime = await _fileSystemProvider.File.GetLastWriteTimeAsync(
                    absolutePathString,
                    CancellationToken.None)
                .ConfigureAwait(false);
        }

        if (onAfterSaveCompletedWrittenDateTimeFunc is not null)
            await onAfterSaveCompletedWrittenDateTimeFunc.Invoke(fileLastWriteTime);
    }
}
