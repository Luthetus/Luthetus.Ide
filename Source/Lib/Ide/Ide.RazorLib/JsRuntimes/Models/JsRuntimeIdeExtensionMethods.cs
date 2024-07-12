using Microsoft.JSInterop;

namespace Luthetus.Ide.RazorLib.JsRuntimes.Models;

public static class JsRuntimeIdeExtensionMethods
{
    public static JavaScriptInteropIdeApi GetLuthetusIdeApi(this IJSRuntime jsRuntime)
    {
        return new JavaScriptInteropIdeApi(jsRuntime);
    }
}