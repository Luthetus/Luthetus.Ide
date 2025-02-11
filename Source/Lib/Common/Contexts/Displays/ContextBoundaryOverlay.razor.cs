using Luthetus.Common.RazorLib.Contexts.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextBoundaryOverlay : ComponentBase
{
    [Inject]
    private IContextService ContextService { get; set; } = null!;

    [Parameter, EditorRequired]
    public InspectableContext InspectableContext { get; set; } = default!;

    private string GetCssStyleString()
    {
        if (InspectableContext.TargetContextRecordMeasuredHtmlElementDimensions is null)
        {
            /*
             Null reference exception was encountered (2024-05-07)
             -----------------------------------------------------
             Microsoft.AspNetCore.Components.Web.ErrorBoundary[100]
             Unhandled exception rendering component: Object reference not set to an instance of an object.
             System.NullReferenceException: Object reference not set to an instance of an object.
             at Luthetus.Common.RazorLib.Contexts.Displays.ContextBoundary.BuildRenderTree(RenderTreeBuilder __builder)
             at Microsoft.AspNetCore.Components.Rendering.ComponentState.RenderIntoBatch(RenderBatchBuilder batchBuilder, RenderFragment renderFragment, Exception& renderFragmentException)
             */
            return string.Empty;
        }

        var width = $"width: {InspectableContext.TargetContextRecordMeasuredHtmlElementDimensions.WidthInPixels}px;";
        var height = $"height: {InspectableContext.TargetContextRecordMeasuredHtmlElementDimensions.HeightInPixels}px;";
        var left = $"left: {InspectableContext.TargetContextRecordMeasuredHtmlElementDimensions.LeftInPixels}px;";
        var top = $"top: {InspectableContext.TargetContextRecordMeasuredHtmlElementDimensions.TopInPixels}px;";
        var zIndex = $"z-index: {InspectableContext.TargetContextRecordMeasuredHtmlElementDimensions.ZIndex};";

        return $"{width} {height} {left} {top} {zIndex}";
    }

    private void DispatchSetInspectionTargetActionOnClick()
    {
        ContextService.ReduceSetInspectedContextHeirarchyAction(
            InspectableContext.ContextHeirarchy);
    }
}