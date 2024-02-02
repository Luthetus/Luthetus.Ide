using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Storages.States;

public partial class StorageSync
{
    public void WriteToLocalStorage(
        string key,
        object value)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "WriteToStorage",
            async () => 
            {
                var valueJson = System.Text.Json.JsonSerializer.Serialize(value);
                await _storageService.SetValue(key, valueJson).ConfigureAwait(false);
            });
    }
}