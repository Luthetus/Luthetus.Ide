using Fluxor;
using Microsoft.JSInterop;

namespace Luthetus.Ide.RazorLib.LocalStorageCase.Models;

public class LocalStorageEffects
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageEffects(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public record LocalStorageSetItemAction(string Key, string Value);

    [EffectMethod]
    public async Task HandleLocalStorageSetItemAction(
        LocalStorageSetItemAction localStorageSetItemAction,
        IDispatcher dispatcher)
    {
        await _jsRuntime.InvokeVoidAsync(
            "luthetusIde.localStorageSetItem",
            localStorageSetItemAction.Key,
            localStorageSetItemAction.Value);
    }
}