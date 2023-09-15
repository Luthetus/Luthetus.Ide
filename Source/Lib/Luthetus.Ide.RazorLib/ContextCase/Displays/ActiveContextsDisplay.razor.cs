using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.RazorLib.ContextCase.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.ContextCase.Displays;

public partial class ActiveContextsDisplay : FluxorComponent
{
    [Inject]
    private IState<ContextRegistry> ContextStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private bool GetIsInspecting(ContextRegistry localContextStates) =>
        localContextStates.InspectionTargetContextRecords is not null;

    private void DispatchToggleInspectActionOnClick(bool isInspecting)
    {
        if (isInspecting)
        {
            Dispatcher.Dispatch(new ContextRegistry.SetSelectInspectionTargetFalseAction());
        }
        else
        {
            Dispatcher.Dispatch(new ContextRegistry.SetSelectInspectionTargetTrueAction());
        }
    }
}