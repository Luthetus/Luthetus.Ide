using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

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
        CommonApi.ContextApi.ContextStateChanged += OnContextStateChanged;
		base.OnInitialized();
	}

    protected override bool ShouldRender()
    {
        var contextState = CommonApi.ContextApi.GetContextState();

        if (_previousIsSelectingInspectionTarget != contextState.IsSelectingInspectionTarget)
            return true;

        return false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var contextState = CommonApi.ContextApi.GetContextState();

        if (_previousIsSelectingInspectionTarget != contextState.IsSelectingInspectionTarget)
        {
            _previousIsSelectingInspectionTarget = contextState.IsSelectingInspectionTarget;

            if (contextState.IsSelectingInspectionTarget)
            {
                var measuredHtmlElementDimensions = await CommonApi.LuthetusCommonJavaScriptInteropApi
                    .MeasureElementById(ContextRecord.ContextElementId)
                    .ConfigureAwait(false);
                
                var contextBoundaryHeirarchy = GetContextBoundaryHeirarchy.Invoke();

                measuredHtmlElementDimensions = measuredHtmlElementDimensions with
                {
                    ZIndex = contextBoundaryHeirarchy.Count
                };

                CommonApi.ContextApi.ReduceAddInspectableContextAction(
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
        CommonApi.ContextApi.ContextStateChanged -= OnContextStateChanged;
    }
}