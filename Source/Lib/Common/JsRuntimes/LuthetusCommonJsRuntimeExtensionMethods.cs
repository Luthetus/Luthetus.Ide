using Microsoft.JSInterop;

namespace Luthetus.Common.RazorLib.JsRuntimes;

public static class LuthetusCommonJsRuntimeExtensionMethods
{
    public static LuthetusCommonJavaScriptInteropApi GetLuthetusCommonApi(this IJSRuntime jsRuntime)
    {
        return new LuthetusCommonJavaScriptInteropApi(jsRuntime);
    }
}