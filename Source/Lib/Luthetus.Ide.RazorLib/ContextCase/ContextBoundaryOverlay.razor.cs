using Fluxor;
using Luthetus.Ide.ClassLib.Context;
using Luthetus.Ide.ClassLib.JavaScriptObjects;
using Luthetus.Ide.ClassLib.Store.ContextCase;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.ContextCase;

public partial class ContextBoundaryOverlay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public ContextRecord ContextRecord { get; set; } = null!;
    [Parameter, EditorRequired]
    public ImmutableArray<ContextRecord> ContextBoundaryHeirarchy { get; set; }
    [Parameter, EditorRequired]
    public MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions { get; set; } = null!;

    private string GetCssStyleString()
    {
        var width = $"width: {MeasuredHtmlElementDimensions.WidthInPixels}px;";
        var height = $"height: {MeasuredHtmlElementDimensions.HeightInPixels}px;";
        var left = $"left: {MeasuredHtmlElementDimensions.LeftInPixels}px;";
        var top = $"top: {MeasuredHtmlElementDimensions.TopInPixels}px;";
        var zIndex = $"z-index: {MeasuredHtmlElementDimensions.ZIndex};";

        return $"{width} {height} {left} {top} {zIndex}";
    }

    private void DispatchSetInspectionTargetActionOnClick()
    {
        Dispatcher.Dispatch(new ContextStates.SetInspectionTargetAction(
            ContextBoundaryHeirarchy));
    }
}