using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Options.Models;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextsPanelDisplay : ComponentBase
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IContextService ContextService { get; set; } = null!;

    private bool GetIsInspecting(ContextState localContextStates) =>
        localContextStates.InspectedContextHeirarchy is not null;

    private void DispatchToggleInspectActionOnClick(bool isInspecting)
    {
        if (isInspecting)
            ContextService.ReduceIsSelectingInspectableContextHeirarchyAction(false);
        else
            ContextService.ReduceIsSelectingInspectableContextHeirarchyAction(true);
    }
}