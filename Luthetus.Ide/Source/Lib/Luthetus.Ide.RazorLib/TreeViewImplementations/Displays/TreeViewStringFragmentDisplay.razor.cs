using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Displays;

public partial class TreeViewStringFragmentDisplay : FluxorComponent
{
	[Inject]
    private IStateSelection<TerminalSessionState, TerminalSession?> TerminalSessionsStateSelection { get; set; } = null!;
	[Inject]
    private IState<TerminalSessionState> TerminalSessionStateWrap { get; set; } = null!;
	[Inject]
    private IState<TerminalSessionWasModifiedState> TerminalSessionWasModifiedStateWrap { get; set; } = null!;

	[Parameter, EditorRequired]
	public TreeViewStringFragment TreeViewStringFragment { get; set; } = null!;

	protected override void OnInitialized()
    {
        // Supress un-used services
		{
			_ = TerminalSessionStateWrap;
			_ = TerminalSessionWasModifiedStateWrap;
		}

        TerminalSessionsStateSelection.Select(x =>
        {
            if (x.TerminalSessionMap.TryGetValue(
					TerminalSessionFacts.EXECUTION_TERMINAL_SESSION_KEY,
					out var terminalSession))
			{
				return terminalSession;
			}

            return null;
        });

        base.OnInitialized();
    }
}