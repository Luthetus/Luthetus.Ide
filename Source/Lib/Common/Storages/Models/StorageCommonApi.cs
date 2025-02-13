using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Storages.Models;

public class StorageCommonApi
{
    private readonly LuthetusCommonApi _commonApi;

    public StorageCommonApi(LuthetusCommonApi commonApi)
    {
        _commonApi = commonApi;;
    }

    public void WriteToLocalStorage(string key, object value)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            "WriteToStorage",
            async () =>
            {
                var valueJson = System.Text.Json.JsonSerializer.Serialize(value);
                await _storageService.SetValue(key, valueJson).ConfigureAwait(false);
            });
    }
}
