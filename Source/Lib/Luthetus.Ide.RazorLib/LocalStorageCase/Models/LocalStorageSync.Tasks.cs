using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;
using Microsoft.JSInterop;

namespace Luthetus.Ide.RazorLib.LocalStorageCase.Models;

public partial class LocalStorageSync
{
    public void ReduceLocalStorageSetItemTask(string key, string value)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "RefreshGit",
            async () => await LocalStorageSetItemAsync(key, value));
    }

    private async Task LocalStorageSetItemAsync(string key, string value)
    {
        await _jsRuntime.InvokeVoidAsync("luthetusIde.localStorageSetItem",
            key,
            value);
    }
}