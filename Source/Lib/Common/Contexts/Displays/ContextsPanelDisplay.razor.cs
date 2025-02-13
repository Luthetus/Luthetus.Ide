using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Options.Models;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextsPanelDisplay : ComponentBase, IDisposable
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;

	protected override void OnInitialized()
    {
        ContextService.ContextStateChanged += OnContextStateChanged;
        AppOptionsService.AppOptionsStateChanged += OnAppOptionsStateChanged;
        base.OnInitialized();
    }

    private bool GetIsInspecting(ContextState localContextStates) =>
        localContextStates.InspectedContextHeirarchy is not null;

    private void DispatchToggleInspectActionOnClick(bool isInspecting)
    {
        if (isInspecting)
            ContextService.ReduceIsSelectingInspectableContextHeirarchyAction(false);
        else
            ContextService.ReduceIsSelectingInspectableContextHeirarchyAction(true);
    }
    
    private async void OnContextStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    private async void OnAppOptionsStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	ContextService.ContextStateChanged -= OnContextStateChanged;
    	AppOptionsService.AppOptionsStateChanged -= OnAppOptionsStateChanged;
    }
}