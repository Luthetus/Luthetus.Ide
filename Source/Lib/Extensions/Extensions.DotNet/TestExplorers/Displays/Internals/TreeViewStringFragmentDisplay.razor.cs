using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.Displays.Internals;

public partial class TreeViewStringFragmentDisplay : ComponentBase, IDisposable
{
	[Inject]
	private ITerminalService TerminalService { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

	[Parameter, EditorRequired]
	public TreeViewStringFragment TreeViewStringFragment { get; set; } = null!;

	protected override void OnInitialized()
	{
		TerminalService.TerminalStateChanged += OnTerminalStateChanged;
		base.OnInitialized();
	}

	private string? GetTerminalCommandRequestOutput(ITerminal terminal)
	{
		return TreeViewStringFragment.Item.TerminalCommandParsed?.OutputCache.ToString() ?? null;
	}
	
	private async void OnTerminalStateChanged()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	public void Dispose()
	{
		TerminalService.TerminalStateChanged -= OnTerminalStateChanged;
	}
}