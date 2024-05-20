using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.Displays;

public partial class TreeViewStringFragmentDisplay : FluxorComponent
{
	[Inject]
    private IStateSelection<TerminalState, Terminal?> TerminalStateSelection { get; set; } = null!;
    [Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;

    [Parameter, EditorRequired]
	public TreeViewStringFragment TreeViewStringFragment { get; set; } = null!;

    protected override void OnInitialized()
    {
        // Supress un-used service, because I'm hackily injecting it so that 'FluxorComponent'
        // subscribes to its state changes, even though in this class its "unused".
        _ = TerminalStateWrap;

        TerminalStateSelection.Select(x =>
        {
            if (x.TerminalMap.TryGetValue(
                TerminalFacts.EXECUTION_TERMINAL_KEY,
                out var terminal))
			{
				return terminal;
			}

            return null;
        });

        base.OnInitialized();
    }
}