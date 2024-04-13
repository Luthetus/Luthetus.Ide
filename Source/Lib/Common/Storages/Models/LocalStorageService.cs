using Microsoft.JSInterop;

namespace Luthetus.Common.RazorLib.Storages.Models;

public class LocalStorageService : IStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async ValueTask SetValue(string key, object? value)
    {
        await _jsRuntime.InvokeVoidAsync(
                "luthetusCommon.localStorageSetItem",
                key,
                value)
			.ConfigureAwait(false);
    }

    public async ValueTask<object?> GetValue(string key)
    {
        return await _jsRuntime.InvokeAsync<string>(
                "luthetusCommon.localStorageGetItem",
                key)
			.ConfigureAwait(false);
    }
}