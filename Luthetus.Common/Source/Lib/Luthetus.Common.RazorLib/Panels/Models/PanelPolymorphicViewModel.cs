using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Fluxor;
using Microsoft.JSInterop;

namespace Luthetus.Common.RazorLib.Panels.Models;

public class PanelPolymorphicViewModel : IPolymorphicViewModel
{
	public PanelPolymorphicViewModel(
		Key<Panel> panelKey,
		Key<PanelGroup> panelGroupKey,
		IState<PanelsState> panelsStateWrap,
		IDispatcher dispatcher,
		IDialogService dialogService,
		IJSRuntime? jsRuntime)
	{
		PanelKey = panelKey;
		PanelGroupKey = panelGroupKey;
		PanelsStateWrap = panelsStateWrap;
		Dispatcher = dispatcher;
		DialogService = dialogService;
		JsRuntime = jsRuntime;

		TabViewModel = new PanelTabViewModel(
			PanelKey,
			PanelGroupKey,
			PanelsStateWrap,
			Dispatcher,
			this);

		DraggableViewModel = new PanelDraggableViewModel(
			PanelKey,
			PanelGroupKey,
			PanelsStateWrap,
			Dispatcher,
			DialogService,
			JsRuntime,
			this);

		DialogViewModel = new PanelDialogViewModel(
			PanelKey,
			PanelGroupKey,
			PanelsStateWrap,
			Dispatcher,
			DialogService,
			JsRuntime,
			this);
	}

	public Key<Panel> PanelKey { get; }
	public Key<PanelGroup> PanelGroupKey { get; }
	public IState<PanelsState> PanelsStateWrap { get; }
	public IDispatcher Dispatcher { get; }
	public IDialogService DialogService { get; }
	public IJSRuntime? JsRuntime { get; }

	public IDialogViewModel? DialogViewModel { get; init; }
    public IDraggableViewModel? DraggableViewModel { get; init; }
    public IDropzoneViewModel? DropzoneViewModel { get; init; }
    public INotificationViewModel? NotificationViewModel { get; init; }
    public ITabViewModel? TabViewModel { get; init; }
}
