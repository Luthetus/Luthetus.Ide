using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Storages.Models;

public class LuthetusCommonStorageBackgroundTaskApi
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IStorageService _storageService;

    public LuthetusCommonStorageBackgroundTaskApi(
        IBackgroundTaskService backgroundTaskService,
        IStorageService storageService)
    {
        _backgroundTaskService = backgroundTaskService;
        _storageService = storageService;
    }

    public Task WriteToLocalStorage(string key, object value)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "WriteToStorage",
            async () =>
            {
                var valueJson = System.Text.Json.JsonSerializer.Serialize(value);
                await _storageService.SetValue(key, valueJson).ConfigureAwait(false);
            });
    }
}
