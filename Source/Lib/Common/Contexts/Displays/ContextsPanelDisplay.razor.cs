using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Contexts.States;
using Luthetus.Common.RazorLib.Options.Models;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextsPanelDisplay : FluxorComponent
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IState<ContextState> ContextStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private bool GetIsInspecting(ContextState localContextStates) =>
        localContextStates.InspectedContextHeirarchy is not null;

    private void DispatchToggleInspectActionOnClick(bool isInspecting)
    {
        if (isInspecting)
            Dispatcher.Dispatch(new ContextState.IsSelectingInspectableContextHeirarchyAction(false));
        else
            Dispatcher.Dispatch(new ContextState.IsSelectingInspectableContextHeirarchyAction(true));
    }
}