using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextsPanelDisplay : ComponentBase, IDisposable
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;

	protected override void OnInitialized()
    {
        CommonApi.ContextApi.ContextStateChanged += OnContextStateChanged;
        CommonApi.AppOptionApi.AppOptionsStateChanged += OnAppOptionsStateChanged;
        base.OnInitialized();
    }

    private bool GetIsInspecting(ContextState localContextStates) =>
        localContextStates.InspectedContextHeirarchy is not null;

    private void DispatchToggleInspectActionOnClick(bool isInspecting)
    {
        if (isInspecting)
            CommonApi.ContextApi.ReduceIsSelectingInspectableContextHeirarchyAction(false);
        else
            CommonApi.ContextApi.ReduceIsSelectingInspectableContextHeirarchyAction(true);
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
        CommonApi.ContextApi.ContextStateChanged -= OnContextStateChanged;
        CommonApi.AppOptionApi.AppOptionsStateChanged -= OnAppOptionsStateChanged;
    }
}