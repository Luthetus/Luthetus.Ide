using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.ClassLib.Store.ContextCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.ContextCase;

public partial class ActiveContextsDisplay : FluxorComponent
{
    [Inject]
    private IState<ContextStates> ContextStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private bool GetIsInspecting(ContextStates localContextStates) =>
        localContextStates.InspectionTargetContextRecords is not null;

    private void DispatchToggleInspectActionOnClick()
    {
        Dispatcher.Dispatch(new ContextStates.ToggleSelectInspectionTargetAction());
    }
}