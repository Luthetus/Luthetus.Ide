using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.ContextCase;

public partial class ContextInitializerDisplay : FluxorComponent
{
    [Inject]
    private IState<ContextRegistry> ContextStatesWrap { get; set; } = null!;
}
