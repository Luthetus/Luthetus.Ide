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
    public InspectContextRecordEntry InspectContextRecordEntry { get; set; } = null!;

    private string GetCssStyleString()
    {
        var width = $"width: {InspectContextRecordEntry.TargetContextRecordMeasuredHtmlElementDimensions.WidthInPixels}px;";
        var height = $"height: {InspectContextRecordEntry.TargetContextRecordMeasuredHtmlElementDimensions.HeightInPixels}px;";
        var left = $"left: {InspectContextRecordEntry.TargetContextRecordMeasuredHtmlElementDimensions.LeftInPixels}px;";
        var top = $"top: {InspectContextRecordEntry.TargetContextRecordMeasuredHtmlElementDimensions.TopInPixels}px;";
        var zIndex = $"z-index: {InspectContextRecordEntry.TargetContextRecordMeasuredHtmlElementDimensions.ZIndex};";

        return $"{width} {height} {left} {top} {zIndex}";
    }

    private void DispatchSetInspectionTargetActionOnClick()
    {
        Dispatcher.Dispatch(new ContextState.SetInspectionTargetAction(
            InspectContextRecordEntry.TargetContextRecordKeyAndHeirarchyBag));
    }
}