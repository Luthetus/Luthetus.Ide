using System.Collections.Concurrent;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.FileSystems.Models;

public class FileSystemIdeApi : IBackgroundTaskGroup
{
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly BackgroundTaskService _backgroundTaskService;
    private readonly INotificationService _notificationService;

    public FileSystemIdeApi(
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        IFileSystemProvider fileSystemProvider,
        ICommonComponentRenderers commonComponentRenderers,
        BackgroundTaskService backgroundTaskService,
        INotificationService notificationService)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _fileSystemProvider = fileSystemProvider;
        _commonComponentRenderers = commonComponentRenderers;
        _backgroundTaskService = backgroundTaskService;
        _notificationService = notificationService;
    }

    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly ConcurrentQueue<FileSystemIdeApiWorkArgs> _workQueue = new();

    public void Enqueue(FileSystemIdeApiWorkArgs workArgs)
    {
        _workQueue.Enqueue(workArgs);
        _backgroundTaskService.Continuous_EnqueueGroup(this);
    }

    private async ValueTask Do_SaveFile(
        AbsolutePath absolutePath,
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
            NotificationHelper.DispatchInformative("Save Action", "File not found. TODO: Save As", _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(7));
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

    public ValueTask HandleEvent()
    {
        if (!_workQueue.TryDequeue(out FileSystemIdeApiWorkArgs workArgs))
            return ValueTask.CompletedTask;

        switch (workArgs.WorkKind)
        {
            case FileSystemIdeApiWorkKind.SaveFile:
                return Do_SaveFile(workArgs.AbsolutePath, workArgs.Content, workArgs.OnAfterSaveCompletedWrittenDateTimeFunc, workArgs.CancellationToken);
            default:
                Console.WriteLine($"{nameof(FileSystemIdeApi)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
        }
    }
}
