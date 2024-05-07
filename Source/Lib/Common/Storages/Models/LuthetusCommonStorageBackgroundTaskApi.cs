using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Storages.Models;

public class LuthetusCommonStorageBackgroundTaskApi
{
    private readonly IBackgroundTaskService _backgroundTaskService;

    public LuthetusCommonStorageBackgroundTaskApi(IBackgroundTaskService backgroundTaskService)
    {
        _backgroundTaskService = backgroundTaskService;
    }

    public Task WriteToLocalStorage(IStorageService storageService, string key, object value)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "WriteToStorage",
            async () =>
            {
                var valueJson = System.Text.Json.JsonSerializer.Serialize(value);
                await storageService.SetValue(key, valueJson).ConfigureAwait(false);
            });
    }
}
