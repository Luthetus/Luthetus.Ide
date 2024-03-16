using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Panels.Displays;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Tabs.Displays;
using Fluxor;
using Microsoft.JSInterop;

namespace Luthetus.Common.RazorLib.Panels.Models;

public record PanelDialogViewModel : IDialogViewModel
{
	public PanelDialogViewModel(
		Key<Panel> panelKey,
		Key<PanelGroup> panelGroupKey,
		IState<PanelsState> panelsStateWrap,
		IDispatcher dispatcher,
		IDialogService dialogService,
		IJSRuntime jsRuntime,
		IPolymorphicViewModel? polymorphicViewModel)
	{
		PanelKey = panelKey;
		PanelGroupKey = panelGroupKey;
		PanelsStateWrap = panelsStateWrap;
		Dispatcher = dispatcher;
		DialogService = dialogService;
		JsRuntime = jsRuntime;
		PolymorphicViewModel = polymorphicViewModel;

		Key = new(PanelKey.Guid);
	}

	public Key<Panel> PanelKey { get; init; }
	public Key<PanelGroup> PanelGroupKey { get; init; }
	public IState<PanelsState> PanelsStateWrap { get; init; }
	public IDispatcher Dispatcher { get; init; }
	public IDialogService DialogService { get; init; }
	public IJSRuntime JsRuntime { get; init; }

	public IPolymorphicViewModel? PolymorphicViewModel { get; init; }
	public Key<IDialogViewModel> Key { get; init; }
	public Type RendererType => GetRendererType();
	public string Title => GetTitle();
	public Dictionary<string, object?>? ParameterMap { get; init; }
	public ElementDimensions ElementDimensions { get; init; } = IDialogViewModel.ConstructDefaultElementDimensions();
    public bool IsMinimized { get; init; }
    public bool IsMaximized { get; init; }
    public bool IsResizable { get; init; } = true;
    public string CssClassString { get; init; }
    public string FocusPointHtmlElementId => $"luth_dialog-focus-point_{Key.Guid}";

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

	public Type GetRendererType()
	{
		var panelGroup = GetPanelGroup();
		var panel = GetPanel(panelGroup);
		
		if (panelGroup is null || panel is null)
			return typeof(PanelErrorDisplay);

		return panel.ContentRendererType;
	}

	public IDialogViewModel SetParameterMap(Dictionary<string, object?>? parameterMap)
	{
		return this with { ParameterMap = parameterMap };
	}

	public IDialogViewModel SetTitle(string title)
	{
		// TODO: How to handle SetTitle?
		return this;
	}

	public IDialogViewModel SetIsMinimized(bool isMinimized)
	{
		return this with { IsMinimized = isMinimized };
	}
	
	public IDialogViewModel SetIsMaximized(bool isMaximized)
	{
		return this with { IsMaximized = isMaximized };
	}

	public IDialogViewModel SetIsResizable(bool isResizable)
	{
		return this with { IsResizable = isResizable };
	}

	public IDialogViewModel SetCssClassString(string cssClassString)
	{
		return this with { CssClassString = cssClassString };
	}
}
