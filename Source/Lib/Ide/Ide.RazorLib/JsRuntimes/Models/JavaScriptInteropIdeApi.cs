using Microsoft.JSInterop;

namespace Luthetus.Ide.RazorLib.JsRuntimes.Models;

public class JavaScriptInteropIdeApi
{
    private readonly IJSRuntime _jsRuntime;

    public JavaScriptInteropIdeApi(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
}
