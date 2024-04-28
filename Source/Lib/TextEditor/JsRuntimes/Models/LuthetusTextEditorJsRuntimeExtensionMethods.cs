using Microsoft.JSInterop;

namespace Luthetus.TextEditor.RazorLib.JsRuntimes.Models;

public static class LuthetusTextEditorJsRuntimeExtensionMethods
{
    public static LuthetusTextEditorJavaScriptInteropApi GetLuthetusTextEditorApi(this IJSRuntime jsRuntime)
    {
        return new LuthetusTextEditorJavaScriptInteropApi(jsRuntime);
    }
}