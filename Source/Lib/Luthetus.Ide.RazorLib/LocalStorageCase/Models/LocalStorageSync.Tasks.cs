using Microsoft.JSInterop;
using static Luthetus.Ide.RazorLib.LocalStorageCase.Models.LocalStorageState;

namespace Luthetus.Ide.RazorLib.LocalStorageCase.Models;

public partial class LocalStorageSync
{
    public async Task LocalStorageSetItem(
        LocalStorageSetItemTask localStorageSetItemTask)
    {
        await _jsRuntime.InvokeVoidAsync(
            "luthetusIde.localStorageSetItem",
            localStorageSetItemTask.Key,
            localStorageSetItemTask.Value);
    }
}