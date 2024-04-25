using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.Virtualizations.Displays;

namespace Luthetus.TextEditor.RazorLib.JsRuntimes;

public class LuthetusTextEditorJavaScriptInteropApi
{
    private readonly IJSRuntime _jsRuntime;

    public LuthetusTextEditorJavaScriptInteropApi(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask FocusHtmlElementById(string elementId)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusTextEditor.focusHtmlElementById",
            elementId);
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

    public ValueTask<int> CalculateProportionalColumnIndex(
        string containerElementId,
        string parentElementId,
        string cursorElementId,
        double positionXInPixels,
        double characterWidthInPixels,
        string textOnRow)
    {
        return _jsRuntime.InvokeAsync<int>(
            "luthetusTextEditor.calculateProportionalColumnIndex",
            containerElementId,
            parentElementId,
            cursorElementId,
            positionXInPixels,
            characterWidthInPixels,
            textOnRow);
    }

    public ValueTask<double> CalculateProportionalLeftOffset(
        string containerElementId,
        string parentElementId,
        string cursorElementId,
        string textOffsettingCursor,
        bool shouldCreateElements)
    {
        return _jsRuntime.InvokeAsync<double>(
            "luthetusTextEditor.calculateProportionalLeftOffset",
            containerElementId,
            parentElementId,
            cursorElementId,
            textOffsettingCursor,
            shouldCreateElements);
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

    public ValueTask MutateScrollVerticalPositionByPixels(
        string textEditorBodyId,
        string gutterElementId,
        double pixels)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusTextEditor.mutateScrollVerticalPositionByPixels",
            textEditorBodyId,
            gutterElementId,
            pixels);
    }

    public ValueTask MutateScrollHorizontalPositionByPixels(
        string bodyElementId,
        string gutterElementId,
        double pixels)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusTextEditor.mutateScrollHorizontalPositionByPixels",
            bodyElementId,
            gutterElementId,
            pixels);
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

    /// <summary>
    /// TODO: This javascript function is only invoked by other javascript functions.
    /// </summary>
    public ValueTask ValidateTextEditorBodyScrollPosition(
        string bodyElementId,
        string gutterElementId,
        double? scrollLeftInPixels,
        double? scrollTopInPixels)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusTextEditor.validateTextEditorBodyScrollPosition",
            bodyElementId,
            gutterElementId,
            scrollLeftInPixels,
            scrollTopInPixels);
    }

    public ValueTask SetGutterScrollTop(
        string gutterElementId,
        double scrollTopInPixels)
    {
        return _jsRuntime.InvokeVoidAsync(
            "luthetusTextEditor.setGutterScrollTop",
            gutterElementId,
            scrollTopInPixels);
    }

    public ValueTask<TextEditorMeasurements> GetTextEditorMeasurementsInPixelsById(
        string elementId)
    {
        return _jsRuntime.InvokeAsync<TextEditorMeasurements>(
            "luthetusTextEditor.getTextEditorMeasurementsInPixelsById",
            elementId);
    }

    /// <summary>
    /// TODO: This javascript function is only invoked by other javascript functions.
    /// </summary>
    public ValueTask<TextEditorMeasurements> GetElementMeasurementsInPixelsByElementReference(ElementReference elementReference)
    {
        // TODO: Not sure if one can pass a C# 'ElementReference' like this to JS interop
        return _jsRuntime.InvokeAsync<TextEditorMeasurements>(
            "luthetusTextEditor.getElementMeasurementsInPixelsByElementReference",
            elementReference);
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
