using Luthetus.Common.RazorLib.JsRuntimes.Models;

namespace Luthetus.Ide.RazorLib.LocalStorages.Models;

public partial class LocalStorageSync
{
    private async Task LocalStorageSetItemAsync(string key, string value)
    {
        await _jsRuntime.GetLuthetusCommonApi()
            .LocalStorageSetItem(
                key,
                value)
            .ConfigureAwait(false);
    }
}