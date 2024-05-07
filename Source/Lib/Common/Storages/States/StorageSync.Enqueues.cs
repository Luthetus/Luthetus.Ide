using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Storages.States;

public partial class StorageSync
{
    public Task WriteToLocalStorage(
        string key,
        object value)
    {
        return BackgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "WriteToStorage",
            async () => 
            {
                var valueJson = System.Text.Json.JsonSerializer.Serialize(value);
                await _storageService.SetValue(key, valueJson).ConfigureAwait(false);
            });
    }
}