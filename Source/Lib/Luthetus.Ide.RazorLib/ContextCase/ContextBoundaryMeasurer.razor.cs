using Microsoft.AspNetCore.Components;
using Fluxor.Blazor.Web.Components;
using Fluxor;
using Microsoft.JSInterop;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.JavaScriptObjectsCase;
using Luthetus.Ide.RazorLib.ContextCase;

namespace Luthetus.Ide.RazorLib.ContextCase;

public partial class ContextBoundaryMeasurer : FluxorComponent
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private IState<ContextRegistry> ContextStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public ContextRecord ContextRecord { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<ImmutableArray<ContextRecord>> GetContextBoundaryHeirarchy { get; set; } = null!;

    private bool _previousIsSelectingInspectionTarget;

    protected override bool ShouldRender()
    {
        var contextStates = ContextStatesWrap.Value;

        if (_previousIsSelectingInspectionTarget != contextStates.IsSelectingInspectionTarget)
            return true;

        return false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var contextStates = ContextStatesWrap.Value;

        if (_previousIsSelectingInspectionTarget != contextStates.IsSelectingInspectionTarget)
        {
            _previousIsSelectingInspectionTarget = contextStates.IsSelectingInspectionTarget;

            if (contextStates.IsSelectingInspectionTarget)
            {
                var measuredHtmlElementDimensions = await JsRuntime.InvokeAsync<MeasuredHtmlElementDimensions>("luthetusIde.measureElementById",
                    ContextRecord.ContextElementId);

                var contextBoundaryHeirarchy = GetContextBoundaryHeirarchy.Invoke();

                measuredHtmlElementDimensions = measuredHtmlElementDimensions with
                {
                    ZIndex = contextBoundaryHeirarchy.Length
                };

                Dispatcher.Dispatch(new ContextRegistry.AddMeasuredHtmlElementDimensionsAction(
                    ContextRecord, contextBoundaryHeirarchy, measuredHtmlElementDimensions));
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}