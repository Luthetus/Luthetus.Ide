using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.Displays.Internals;

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

		base.OnInitialized();
	}

	private string? GetTerminalCommandOutput(Terminal terminal)
	{
		return TreeViewStringFragment.Item.TerminalCommand?.OutputBuilder?.ToString() ?? null;
	}
}