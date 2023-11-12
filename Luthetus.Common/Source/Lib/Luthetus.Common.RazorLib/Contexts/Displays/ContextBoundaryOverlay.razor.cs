using Fluxor;
using Luthetus.Common.RazorLib.Contexts.States;
using Luthetus.Common.RazorLib.Contexts.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextBoundaryOverlay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public InspectableContext InspectableContext { get; set; } = null!;

    private string GetCssStyleString()
    {
        var width = $"width: {InspectableContext.TargetContextRecordMeasuredHtmlElementDimensions.WidthInPixels}px;";
        var height = $"height: {InspectableContext.TargetContextRecordMeasuredHtmlElementDimensions.HeightInPixels}px;";
        var left = $"left: {InspectableContext.TargetContextRecordMeasuredHtmlElementDimensions.LeftInPixels}px;";
        var top = $"top: {InspectableContext.TargetContextRecordMeasuredHtmlElementDimensions.TopInPixels}px;";
        var zIndex = $"z-index: {InspectableContext.TargetContextRecordMeasuredHtmlElementDimensions.ZIndex};";

        return $"{width} {height} {left} {top} {zIndex}";
    }

    private void DispatchSetInspectionTargetActionOnClick()
    {
        Dispatcher.Dispatch(new ContextState.SetInspectedContextHeirarchyAction(
            InspectableContext.ContextHeirarchy));
    }
}