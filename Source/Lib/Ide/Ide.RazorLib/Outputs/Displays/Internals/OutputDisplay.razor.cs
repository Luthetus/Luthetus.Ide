using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.States;

namespace Luthetus.Ide.RazorLib.Outputs.Displays.Internals;

public partial class OutputDisplay : FluxorComponent
{
	[Inject]
	private DotNetCliOutputParser DotNetCliOutputParser { get; set; } = null!;
	[Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
}