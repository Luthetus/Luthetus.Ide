using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.DotNetOutputs.Models;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;

namespace Luthetus.Ide.RazorLib.Outputs.Displays;

public partial class OutputPanelDisplay : FluxorComponent
{
    [Inject]
    private IState<WellKnownTerminalSessionsState> WellKnownTerminalSessionsStateWrap { get; set; } = null!;

	private readonly DotNetRunOutputParser _dotNetRunOutputParser = new();
}