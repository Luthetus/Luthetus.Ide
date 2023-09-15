using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.RazorLib.TerminalCase.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TerminalCase.Displays;

public partial class TerminalDisplay : FluxorComponent
{
    [Inject]
    private IState<WellKnownTerminalSessionsRegistry> WellKnownTerminalSessionsStateWrap { get; set; } = null!;
}