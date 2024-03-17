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
    public IDispatcher Dispatcher { get; set; }

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
			Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(Key, Key<Panel>.Empty));
		else
			Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(Key, panelTab.Key));
		
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

		Dispatcher.Dispatch(new PanelsState.DisposePanelTabAction(Key, panelTab.Key));
		return Task.CompletedTask;
	}
}