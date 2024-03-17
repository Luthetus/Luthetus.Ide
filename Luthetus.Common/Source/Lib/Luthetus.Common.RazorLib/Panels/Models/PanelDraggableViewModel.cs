using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Tabs.Displays;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using System.Collections.Immutable;
using Fluxor;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Panels.Models;

public class PanelDraggableViewModel : IDraggableViewModel
{
	public PanelDraggableViewModel(
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

		ParameterMap = new Dictionary<string, object?>
		{
			{
				nameof(TabDisplay.TabViewModel),
				PolymorphicViewModel.TabViewModel
			},
			{
				nameof(TabDisplay.IsBeingDragged),
				true
			}
		};
	}

	public Key<Panel> PanelKey { get; init; }
	public Key<PanelGroup> PanelGroupKey { get; init; }
	public IState<PanelsState> PanelsStateWrap { get; init; }
	public IDispatcher Dispatcher { get; init; }
	public IDialogService DialogService { get; init; }
	public IJSRuntime JsRuntime { get; init; }

	public IPolymorphicViewModel? PolymorphicViewModel { get; init; }
	public Key<IDraggableViewModel> Key { get; init; }
	public Type RendererType { get; init; } = typeof(TabDisplay);

	public Dictionary<string, object?>? ParameterMap { get; }

	public ElementDimensions ElementDimensions { get; init; } = IDialogViewModel.ConstructDefaultElementDimensions();
	public ImmutableArray<IDropzoneViewModel> DropzoneViewModelList { get; set; } = ImmutableArray<IDropzoneViewModel>.Empty;

	public PanelGroup? GetPanelGroup()
	{
		return PanelsStateWrap.Value.PanelGroupList.FirstOrDefault(x => x.Key == PanelGroupKey);
	}

	public Panel? GetPanel(PanelGroup panelGroup)
	{
		return panelGroup.TabList.FirstOrDefault(x => x.Key == PanelKey);
	}

	
}
