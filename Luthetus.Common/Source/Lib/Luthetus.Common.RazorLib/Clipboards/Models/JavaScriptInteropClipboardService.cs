using Microsoft.JSInterop;

namespace Luthetus.Common.RazorLib.Clipboards.Models;

public class JavaScriptInteropClipboardService : IClipboardService
{
    private readonly IJSRuntime _jsRuntime;

    public JavaScriptInteropClipboardService(bool isEnabled, IJSRuntime jsRuntime)
    {
        IsEnabled = isEnabled;
        _jsRuntime = jsRuntime;
    }

    public bool IsEnabled { get; }

    public async Task<string> ReadClipboard()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("luthetusCommon.readClipboard");
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
            await _jsRuntime.InvokeVoidAsync("luthetusCommon.setClipboard", value);
        }
        catch (TaskCanceledException)
        {
        }
    }
}