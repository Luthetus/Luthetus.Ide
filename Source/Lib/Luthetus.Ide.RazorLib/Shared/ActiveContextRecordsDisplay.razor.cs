using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.ClassLib.Store.ContextCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Shared;

public partial class ActiveContextRecordsDisplay : FluxorComponent
{
    [Inject]
    private IState<ContextStates> ContextStatesWrap { get; set; } = null!;
}