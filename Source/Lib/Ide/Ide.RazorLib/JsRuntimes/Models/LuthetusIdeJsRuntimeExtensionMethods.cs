using Microsoft.JSInterop;

namespace Luthetus.Ide.RazorLib.JsRuntimes.Models;

/// <remarks>
/// This class is an exception to the naming convention, "don't use the word 'Luthetus' in class names".
/// 
/// Reason for this exception: the 'IJSRuntime' datatype is far more common in code,
/// 	than some specific type (example: DialogDisplay.razor).
///     So, adding 'Luthetus' in the class name for redundancy seems meaningful here.
/// </remarks>
public static class LuthetusIdeJsRuntimeExtensionMethods
{
    public static LuthetusIdeJavaScriptInteropApi GetLuthetusIdeApi(this IJSRuntime jsRuntime)
    {
        return new LuthetusIdeJavaScriptInteropApi(jsRuntime);
    }
}