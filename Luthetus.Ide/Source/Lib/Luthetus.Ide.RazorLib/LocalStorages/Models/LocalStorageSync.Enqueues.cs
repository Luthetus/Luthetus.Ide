using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.LocalStorages.Models;

public partial class LocalStorageSync
{
    public void LocalStorageSetItem(string key, string value)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "LocalStorage SetItem",
            async () => await LocalStorageSetItemAsync(key, value));
    }
}