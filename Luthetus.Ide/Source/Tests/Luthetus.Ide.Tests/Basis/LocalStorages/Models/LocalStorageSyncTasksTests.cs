using Microsoft.JSInterop;

namespace Luthetus.Ide.Tests.Basis.LocalStorages.Models;

public partial class LocalStorageSyncTasksTests
{
    private async Task LocalStorageSetItemAsync(string key, string value)
    {
        await _jsRuntime.InvokeVoidAsync("luthetusIde.localStorageSetItem",
            key,
            value);
    }
}