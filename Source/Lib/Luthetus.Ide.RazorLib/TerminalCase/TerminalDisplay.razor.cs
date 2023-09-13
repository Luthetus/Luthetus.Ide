using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TerminalCase;

public partial class TerminalDisplay : FluxorComponent
{
    [Inject]
    private IState<WellKnownTerminalSessionsRegistry> WellKnownTerminalSessionsStateWrap { get; set; } = null!;
}