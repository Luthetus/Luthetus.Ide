using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.Virtualizations.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.JsRuntimes.Models;

/// <remarks>
/// This class is an exception to the naming convention, "don't use the word 'Luthetus' in class names".
/// 
/// Reason for this exception: the 'IJSRuntime' datatype is far more common in code,
/// 	than some specific type (example: DialogDisplay.razor).
///     So, adding 'Luthetus' in the class name for redundancy seems meaningful here.
/// </remarks>
public class LuthetusTextEditorJavaScriptInteropApi
{
    private readonly IJSRuntime _jsRuntime;

    public LuthetusTextEditorJavaScriptInteropApi(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask ScrollElementIntoView(string elementId)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusTextEditor.scrollElementIntoView",
            elementId);
    }

    public ValueTask PreventDefaultOnWheelEvents(string elementId)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusTextEditor.preventDefaultOnWheelEvents",
            elementId);
    }

    public ValueTask<CharAndLineMeasurements> GetCharAndLineMeasurementsInPixelsById(
        string measureCharacterWidthAndLineHeightElementId,
        int countOfTestCharacters)
    {
        return _jsRuntime.InvokeAsync<CharAndLineMeasurements>(
            "luthetusTextEditor.getCharAndLineMeasurementsInPixelsById",
            measureCharacterWidthAndLineHeightElementId,
            countOfTestCharacters);
    }

    /// <summary>
    /// TODO: This javascript function is only used from other javascript functions.
    /// </summary>
    public ValueTask<string> EscapeHtml(string input)
    {
        return _jsRuntime.InvokeAsync<string>(
            "luthetusTextEditor.escapeHtml",
            input);
    }

    public ValueTask<RelativeCoordinates> GetRelativePosition(
        string elementId,
        double clientX,
        double clientY)
    {
        return _jsRuntime.InvokeAsync<RelativeCoordinates>(
            "luthetusTextEditor.getRelativePosition",
            elementId,
            clientX,
            clientY);
    }

    public ValueTask SetScrollPosition(
        string bodyElementId,
        string gutterElementId,
        double? scrollLeftInPixels,
        double? scrollTopInPixels)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusTextEditor.setScrollPosition",
            bodyElementId,
            gutterElementId,
            scrollLeftInPixels,
            scrollTopInPixels);
    }

    public ValueTask<TextEditorDimensions> GetTextEditorMeasurementsInPixelsById(
        string elementId)
    {
        return _jsRuntime.InvokeAsync<TextEditorDimensions>(
            "luthetusTextEditor.getTextEditorMeasurementsInPixelsById",
            elementId);
    }

    /// <summary>
    /// TODO: This is a javascript object
    /// </summary>
    public void CursorIntersectionObserverMap()
    {
        /*
         cursorIntersectionObserverMap: new Map(),
         */
    }

	/*
    public ValueTask InitializeTextEditorCursorIntersectionObserver(
        string intersectionObserverMapKey,
        DotNetObjectReference<CursorDisplay> cursorDisplayDotNetObjectReference,
        string scrollableContainerId,
        string cursorDisplayId)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusTextEditor.initializeTextEditorCursorIntersectionObserver",
            intersectionObserverMapKey,
            cursorDisplayDotNetObjectReference,
            scrollableContainerId,
            cursorDisplayId);
    }
    */

    /// <summary>
    /// TODO: This javascript function is NOT BEING USED by anyone at any point
    /// </summary>
    public ValueTask RevealCursor(
        string intersectionObserverMapKey,
        string cursorElementId)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusTextEditor.revealCursor",
            intersectionObserverMapKey,
            cursorElementId);
    }

    public ValueTask DisposeTextEditorCursorIntersectionObserver(
        CancellationToken cancellationToken,
        string intersectionObserverMapKey)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusTextEditor.disposeTextEditorCursorIntersectionObserver",
            cancellationToken,
            intersectionObserverMapKey);
    }

    /// <summary>
    /// TODO: This is a javascript object
    /// </summary>
    public void VirtualizationIntersectionObserverMap()
    {
        /*
         virtualizationIntersectionObserverMap: new Map(),
         */
    }

    public ValueTask InitializeVirtualizationIntersectionObserver(
        string virtualizationDisplayGuidString,
        DotNetObjectReference<VirtualizationDisplay> virtualizationDisplayDotNetObjectReference,
        ElementReference scrollableParentFinderElementReference,
        List<object>? boundaryIdList)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusTextEditor.initializeVirtualizationIntersectionObserver",
            virtualizationDisplayGuidString,
            virtualizationDisplayDotNetObjectReference,
            scrollableParentFinderElementReference,
            boundaryIdList);
    }

    public ValueTask DisposeVirtualizationIntersectionObserver(
        CancellationToken cancellationToken,
        string virtualizationDisplayGuidString)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusTextEditor.disposeVirtualizationIntersectionObserver",
            cancellationToken,
            virtualizationDisplayGuidString);
    }

    public ValueTask<ElementPositionInPixels> GetBoundingClientRect(string primaryCursorContentId)
    {
        return _jsRuntime.InvokeAsync<ElementPositionInPixels>(
            "luthetusTextEditor.getBoundingClientRect",
            primaryCursorContentId);
    }
}
