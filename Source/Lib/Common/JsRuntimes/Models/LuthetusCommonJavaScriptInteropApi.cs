using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.RazorLib.JsRuntimes.Models;

/// <remarks>
/// This class is an exception to the naming convention, "don't use the word 'Luthetus' in class names".
/// 
/// Reason for this exception: the 'IJSRuntime' datatype is far more common in code,
/// 	than some specific type (example: DialogDisplay.razor).
///     So, adding 'Luthetus' in the class name for redundancy seems meaningful here.
/// </remarks>
public class LuthetusCommonJavaScriptInteropApi
{
    private readonly IJSRuntime _jsRuntime;

    public LuthetusCommonJavaScriptInteropApi(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask SubscribeWindowSizeChanged(DotNetObjectReference<BrowserResizeInterop> browserResizeInteropDotNetObjectReference)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusCommon.subscribeWindowSizeChanged",
            browserResizeInteropDotNetObjectReference);
    }

    public ValueTask DisposeWindowSizeChanged()
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusCommon.disposeWindowSizeChanged");
    }
    
    public ValueTask FocusHtmlElementById(string elementId, bool preventScroll = false)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusCommon.focusHtmlElementById",
            elementId,
            preventScroll);
    }

    public ValueTask<bool> TryFocusHtmlElementById(string elementId)
    {
        return _jsRuntime.InvokeAsync<bool>(
            "luthetusCommon.tryFocusHtmlElementById",
            elementId);
    }
    
    public ValueTask<MeasuredHtmlElementDimensions> MeasureElementById(string elementId)
    {
        return _jsRuntime.InvokeAsync<MeasuredHtmlElementDimensions>(
            "luthetusCommon.measureElementById",
            elementId);
    }

    public ValueTask LocalStorageSetItem(string key, object? value)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusCommon.localStorageSetItem",
            key,
            value);
    }

    public ValueTask<string?> LocalStorageGetItem(string key)
    {
        return _jsRuntime.InvokeAsync<string?>(
            "luthetusCommon.localStorageGetItem",
            key);
    }

    public ValueTask<string> ReadClipboard()
    {
        return _jsRuntime.InvokeAsync<string>(
            "luthetusCommon.readClipboard");
    }

    public ValueTask SetClipboard(string value)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusCommon.setClipboard",
            value);
    }

    public ValueTask<ContextMenuFixedPosition> GetTreeViewContextMenuFixedPosition(
        string nodeElementId)
    {
        return _jsRuntime.InvokeAsync<ContextMenuFixedPosition>(
            "luthetusCommon.getTreeViewContextMenuFixedPosition",
            nodeElementId);
    }
}
