using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Contexts.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ActiveContextsDisplay : FluxorComponent
{
    [Inject]
    private IState<ContextState> ContextStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private bool GetIsInspecting(ContextState localContextStates) =>
        localContextStates.InspectedKeyHeirarchyBag is not null;

    private void DispatchToggleInspectActionOnClick(bool isInspecting)
    {
        if (isInspecting)
            Dispatcher.Dispatch(new ContextState.SetSelectInspectionTargetFalseAction());
        else
            Dispatcher.Dispatch(new ContextState.SetSelectInspectionTargetTrueAction());
    }
}