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
    public Key<BackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public string Name { get; } = nameof(FileSystemIdeApi);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<FileSystemIdeApiWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();

    private readonly
        Queue<(AbsolutePath absolutePath, string content, Func<DateTime?, Task> onAfterSaveCompletedWrittenDateTimeFunc, CancellationToken cancellationToken)>
        _queue_SaveFile = new();

    public void Enqueue_SaveFile(
        AbsolutePath absolutePath,
        string content,
        Func<DateTime?, Task> onAfterSaveCompletedWrittenDateTimeFunc,
        CancellationToken cancellationToken = default)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(FileSystemIdeApiWorkKind.SaveFile);
            _queue_SaveFile.Enqueue((absolutePath, content, onAfterSaveCompletedWrittenDateTimeFunc, cancellationToken));
            _backgroundTaskService.EnqueueGroup(this);
        }
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

    public IBackgroundTaskGroup? EarlyBatchOrDefault(IBackgroundTaskGroup oldEvent)
    {
        return null;
    }

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        FileSystemIdeApiWorkKind workKind;

        lock (_workLock)
        {
            if (!_workKindQueue.TryDequeue(out workKind))
                return ValueTask.CompletedTask;
        }

        switch (workKind)
        {
            case FileSystemIdeApiWorkKind.SaveFile:
            {
                var args = _queue_SaveFile.Dequeue();
                return Do_SaveFile(args.absolutePath, args.content, args.onAfterSaveCompletedWrittenDateTimeFunc, args.cancellationToken);
            }
            default:
            {
                Console.WriteLine($"{nameof(FileSystemIdeApi)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
            }
        }
    }
}
