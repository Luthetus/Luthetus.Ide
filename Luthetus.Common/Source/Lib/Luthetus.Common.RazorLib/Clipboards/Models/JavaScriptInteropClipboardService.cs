using Microsoft.JSInterop;

namespace Luthetus.Common.RazorLib.Clipboards.Models;

public class JavaScriptInteropClipboardService : IClipboardService
{
    private readonly IJSRuntime _jsRuntime;

    public JavaScriptInteropClipboardService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string> ReadClipboard()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("luthetusCommon.readClipboard").ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            return string.Empty;
        }
    }

    public async Task SetClipboard(string value)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("luthetusCommon.setClipboard", value).ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
        }
    }
}