using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.Panels.States;

namespace Luthetus.Common.RazorLib.Panels.Models;

public record PanelGroup(
	    Key<PanelGroup> Key,
	    Key<Panel> ActiveTabKey,
	    ElementDimensions ElementDimensions,
	    ImmutableArray<IPanelTab> TabList)
	: ITabGroup
{
    /// <summary>
    /// TODO: Make this property immutable. Until then in a hack needs to be done where this gets set...
	///       ...for Luthetus.Ide this is done in LuthetusIdeInitializer.razor.cs (2024-04-08)
    /// </summary>
    public IDispatcher Dispatcher { get; set; } = null!;

    public bool GetIsActive(ITab tab)
	{
		if (tab is not IPanelTab panelTab)
			return false;

		return ActiveTabKey == panelTab.Key;
	}

	public Task OnClickAsync(ITab tab, MouseEventArgs mouseEventArgs)
	{
		if (tab is not IPanelTab panelTab)
			return Task.CompletedTask;

		if (GetIsActive(tab))
			Dispatcher.Dispatch(new PanelState.SetActivePanelTabAction(Key, Key<Panel>.Empty));
		else
			Dispatcher.Dispatch(new PanelState.SetActivePanelTabAction(Key, panelTab.Key));
		
		return Task.CompletedTask;
	}

	public string GetDynamicCss(ITab tab)
	{
		return string.Empty;
	}

	public Task CloseAsync(ITab tab)
	{
		if (tab is not IPanelTab panelTab)
			return Task.CompletedTask;

		Dispatcher.Dispatch(new PanelState.DisposePanelTabAction(Key, panelTab.Key));
		return Task.CompletedTask;
	}

	public async Task CloseAllAsync()
	{
		var localTabList = TabList;

		foreach (var tab in localTabList)
		{
			await CloseAsync(tab).ConfigureAwait(false);
		}
	}

	public async Task CloseOthersAsync(ITab safeTab)
    {
        var localTabList = TabList;

		if (safeTab is not IPanelTab safePanelTab)
			return;
		
		// Invoke 'OnClickAsync' to set the active tab to the "safe tab"
		// OnClickAsync does not currently use its mouse event args argument.
		await OnClickAsync(safeTab, null);

        foreach (var tab in localTabList)
        {
			var shouldClose = safePanelTab.Key != tab.Key;

			if (shouldClose)
				await CloseAsync(tab).ConfigureAwait(false);
        }
    }
}