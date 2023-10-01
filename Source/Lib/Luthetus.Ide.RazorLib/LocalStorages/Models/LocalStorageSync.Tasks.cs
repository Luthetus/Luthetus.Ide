using Microsoft.JSInterop;

namespace Luthetus.Ide.RazorLib.LocalStorageCase.Models;

public partial class LocalStorageSync
{
    private async Task LocalStorageSetItemAsync(string key, string value)
    {
        await _jsRuntime.InvokeVoidAsync("luthetusIde.localStorageSetItem",
            key,
            value);
    }
}