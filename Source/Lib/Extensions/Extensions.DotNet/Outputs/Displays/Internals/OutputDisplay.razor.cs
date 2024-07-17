using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.States;

namespace Luthetus.Extensions.DotNet.Outputs.Displays.Internals;

public partial class OutputDisplay : FluxorComponent
{
	[Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
}