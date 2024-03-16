using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;

namespace Luthetus.Common.RazorLib.Panels.Models;

public class PanelTabViewModel : ITabViewModel
{
	public PanelTabViewModel(
		Key<Panel> panelKey,
		Key<PanelGroup> panelGroupKey,
		IState<PanelsState> panelsStateWrap,
		IDispatcher dispatcher,
		IPolymorphicViewModel? polymorphicViewModel)
	{
		PanelKey = panelKey;
		PanelGroupKey = panelGroupKey;
		PanelsStateWrap = panelsStateWrap;
		Dispatcher = dispatcher;
		PolymorphicViewModel = polymorphicViewModel;

		Key = new Key<ITabViewModel>(PanelKey.Guid);
	}

	public Key<Panel> PanelKey { get; }
	public Key<PanelGroup> PanelGroupKey { get; }
	public IState<PanelsState> PanelsStateWrap { get; }
	public IDispatcher Dispatcher { get; }

	public IPolymorphicViewModel? PolymorphicViewModel { get; init; }
	public Key<ITabViewModel> Key { get; }
	public string Title => GetTitle();

	public PanelGroup? GetPanelGroup()
	{
		return PanelsStateWrap.Value.PanelGroupList.FirstOrDefault(x => x.Key == PanelGroupKey);
	}

	public Panel? GetPanel(PanelGroup panelGroup)
	{
		return panelGroup.TabList.FirstOrDefault(x => x.Key == PanelKey);
	}

	public string GetTitle()
	{
		var panelGroup = GetPanelGroup();
		var panel = GetPanel(panelGroup);
		
		if (panelGroup is null || panel is null)
			return "__NOT_FOUND__";

		return panel.Title;
	}

	public bool GetIsActive()
	{
		var panelGroup = GetPanelGroup();
		var panel = GetPanel(panelGroup);
		
		if (panelGroup is null || panel is null)
			return false;

		return panelGroup.ActiveTabKey == panel.Key;
	}

	public string GetDynamicCss()
	{
		return string.Empty;
	}

    public Task OnClickAsync(MouseEventArgs mouseEventArgs)
	{
		if (GetIsActive())
			Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(PanelGroupKey, Key<Panel>.Empty));
		else
			Dispatcher.Dispatch(new PanelsState.SetActivePanelTabAction(PanelGroupKey, PanelKey));
		
		return Task.CompletedTask;
	}

	public Task CloseAsync()
	{
		Dispatcher.Dispatch(new PanelsState.DisposePanelTabAction(PanelGroupKey, PanelKey));
		return Task.CompletedTask;
	}
}
