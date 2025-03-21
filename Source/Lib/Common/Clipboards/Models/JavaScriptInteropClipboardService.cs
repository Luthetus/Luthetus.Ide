using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Clipboards.Models;

public class JavaScriptInteropClipboardService : IClipboardService
{
    private readonly CommonBackgroundTaskApi _commonBackgroundTaskApi;

    public JavaScriptInteropClipboardService(CommonBackgroundTaskApi commonBackgroundTaskApi)
    {
        _commonBackgroundTaskApi = commonBackgroundTaskApi;
    }

    public async Task<string> ReadClipboard()
    {
        try
        {
            return await _commonBackgroundTaskApi.JsRuntimeCommonApi
                .ReadClipboard()
                .ConfigureAwait(false);
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
            await _commonBackgroundTaskApi.JsRuntimeCommonApi
                .SetClipboard(value)
                .ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
        }
    }
}