using Microsoft.AspNetCore.Components;
using Fluxor.Blazor.Web.Components;
using Fluxor;
using Microsoft.JSInterop;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Contexts.States;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextBoundaryMeasurer : FluxorComponent
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private IState<ContextState> ContextStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public ContextRecord ContextRecord { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<ImmutableArray<Key<ContextRecord>>> GetContextBoundaryHeirarchy { get; set; } = null!;

    private bool _previousIsSelectingInspectionTarget;

    protected override bool ShouldRender()
    {
        var contextState = ContextStateWrap.Value;

        if (_previousIsSelectingInspectionTarget != contextState.IsSelectingInspectionTarget)
            return true;

        return false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var contextState = ContextStateWrap.Value;

        if (_previousIsSelectingInspectionTarget != contextState.IsSelectingInspectionTarget)
        {
            _previousIsSelectingInspectionTarget = contextState.IsSelectingInspectionTarget;

            if (contextState.IsSelectingInspectionTarget)
            {
                var measuredHtmlElementDimensions = await JsRuntime.InvokeAsync<MeasuredHtmlElementDimensions>(
                    "luthetusIde.measureElementById",
                    ContextRecord.ContextElementId);

                var contextBoundaryHeirarchy = GetContextBoundaryHeirarchy.Invoke();

                measuredHtmlElementDimensions = measuredHtmlElementDimensions with
                {
                    ZIndex = contextBoundaryHeirarchy.Length
                };

                Dispatcher.Dispatch(new ContextState.AddInspectableContextAction(
                    new InspectableContext(
                        new(contextBoundaryHeirarchy),
                        measuredHtmlElementDimensions)));
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}