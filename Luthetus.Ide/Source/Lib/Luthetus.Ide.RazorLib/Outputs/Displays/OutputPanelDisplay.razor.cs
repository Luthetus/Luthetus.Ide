using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.DotNetOutputs.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;

namespace Luthetus.Ide.RazorLib.Outputs.Displays;

public partial class OutputPanelDisplay : FluxorComponent
{
    [Inject]
    private IState<WellKnownTerminalSessionsState> WellKnownTerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private IState<TerminalSessionState> TerminalSessionsStateWrap { get; set; } = null!;

    protected override void OnInitialized()
    {
        // Supress un-used service, because I'm hackily injecting it so that 'FluxorComponent'
        // subscribes to its state changes, even though in this class its "unused".
        _ = TerminalSessionsStateWrap;

        base.OnInitialized();
    }

    private readonly DotNetRunOutputParser _dotNetRunOutputParser = new();
}