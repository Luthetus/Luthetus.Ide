using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.FileSystems.Models;

public class FileSystemIdeApi
{
	private readonly LuthetusCommonApi _commonApi;
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;

    public FileSystemIdeApi(
    	LuthetusCommonApi commonApi,
        IdeBackgroundTaskApi ideBackgroundTaskApi)
    {
    	_commonApi = commonApi;
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
    }

    public void SaveFile(
        AbsolutePath absolutePath,
        string content,
        Func<DateTime?, Task> onAfterSaveCompletedWrittenDateTimeFunc,
        CancellationToken cancellationToken = default)
    {
        _commonApi.BackgroundTaskApi.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            "Save File",
            async () => await SaveFileAsync(
                    absolutePath,
                    content,
                    onAfterSaveCompletedWrittenDateTimeFunc,
                    cancellationToken)
                .ConfigureAwait(false));
    }

    private async Task SaveFileAsync(
        AbsolutePath absolutePath,
        string content,
        Func<DateTime?, Task> onAfterSaveCompletedWrittenDateTimeFunc,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        var absolutePathString = absolutePath.Value;

        if (absolutePathString is not null &&
            await _commonApi.FileSystemProviderApi.File.ExistsAsync(absolutePathString).ConfigureAwait(false))
        {
            await _commonApi.FileSystemProviderApi.File.WriteAllTextAsync(absolutePathString, content).ConfigureAwait(false);
        }
        else
        {
            // TODO: Save As to make new file
            NotificationHelper.DispatchInformative("Save Action", "File not found. TODO: Save As", _commonApi.ComponentRendererApi, _commonApi.NotificationApi, TimeSpan.FromSeconds(7));
        }

        DateTime? fileLastWriteTime = null;

        if (absolutePathString is not null)
        {
            fileLastWriteTime = await _commonApi.FileSystemProviderApi.File.GetLastWriteTimeAsync(
                    absolutePathString,
                    CancellationToken.None)
                .ConfigureAwait(false);
        }

        if (onAfterSaveCompletedWrittenDateTimeFunc is not null)
            await onAfterSaveCompletedWrittenDateTimeFunc.Invoke(fileLastWriteTime);
    }
}
