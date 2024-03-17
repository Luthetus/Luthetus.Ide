using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Panels.Models;

public record PanelGroup(
	    Key<PanelGroup> Key,
	    Key<Panel> ActiveTabKey,
	    ElementDimensions ElementDimensions,
	    ImmutableArray<Panel> TabList)
	: ITabGroup
{
	public bool GetIsActive(ITab tab)
	{
		if (tab is not IPanelTab panelTab)
			return Task.CompletedTask;

		return panelGroup.ActiveTabKey == panelTab.Key;
	}

	public Task OnClickAsync(ITab tab, MouseEventArgs mouseEventArgs)
	{
		if (tab is not IPanelTab panelTab)
			return Task.CompletedTask;

		if (GetIsActive())
			Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(PanelGroupKey, Key<Panel>.Empty));
		else
			Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(PanelGroupKey, panelTab.Key));
		
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

		Dispatcher.Dispatch(new PanelsState.DisposePanelTabAction(PanelGroupKey, panelTab.Key));
		return Task.CompletedTask;
	}
}