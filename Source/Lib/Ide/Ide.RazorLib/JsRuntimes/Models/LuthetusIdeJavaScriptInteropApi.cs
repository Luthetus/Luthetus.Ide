using Microsoft.JSInterop;

namespace Luthetus.Ide.RazorLib.JsRuntimes.Models;

public class LuthetusIdeJavaScriptInteropApi
{
    private readonly IJSRuntime _jsRuntime;

    public LuthetusIdeJavaScriptInteropApi(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
}
