using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextBoundaryMeasurer : ComponentBase, IDisposable
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;

    [Parameter, EditorRequired]
    public ContextRecord ContextRecord { get; set; } = default!;
    [Parameter, EditorRequired]
    public Func<List<Key<ContextRecord>>> GetContextBoundaryHeirarchy { get; set; } = null!;

    private bool _previousIsSelectingInspectionTarget;

	protected override void OnInitialized()
	{
		ContextService.ContextStateChanged += OnContextStateChanged;
		base.OnInitialized();
	}

    protected override bool ShouldRender()
    {
        var contextState = ContextService.GetContextState();

        if (_previousIsSelectingInspectionTarget != contextState.IsSelectingInspectionTarget)
            return true;

        return false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var contextState = ContextService.GetContextState();

        if (_previousIsSelectingInspectionTarget != contextState.IsSelectingInspectionTarget)
        {
            _previousIsSelectingInspectionTarget = contextState.IsSelectingInspectionTarget;

            if (contextState.IsSelectingInspectionTarget)
            {
                var measuredHtmlElementDimensions = await JsRuntime.GetLuthetusCommonApi()
                    .MeasureElementById(ContextRecord.ContextElementId)
                    .ConfigureAwait(false);
                
                var contextBoundaryHeirarchy = GetContextBoundaryHeirarchy.Invoke();

                measuredHtmlElementDimensions = measuredHtmlElementDimensions with
                {
                    ZIndex = contextBoundaryHeirarchy.Count
                };

                ContextService.ReduceAddInspectableContextAction(
                    new InspectableContext(
                        new(contextBoundaryHeirarchy),
                        measuredHtmlElementDimensions));
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }
    
    public async void OnContextStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	ContextService.ContextStateChanged -= OnContextStateChanged;
    }
}