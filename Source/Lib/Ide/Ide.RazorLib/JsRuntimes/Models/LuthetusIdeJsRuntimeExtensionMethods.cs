using Microsoft.JSInterop;

namespace Luthetus.Ide.RazorLib.JsRuntimes.Models;

public static class LuthetusIdeJsRuntimeExtensionMethods
{
    public static LuthetusIdeJavaScriptInteropApi GetLuthetusIdeApi(this IJSRuntime jsRuntime)
    {
        return new LuthetusIdeJavaScriptInteropApi(jsRuntime);
    }
}