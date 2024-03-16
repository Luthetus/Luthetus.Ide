using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using System.Collections.Immutable;
using Fluxor;
using Microsoft.JSInterop;

namespace Luthetus.Common.RazorLib.Panels.Models;

public class PanelDropzoneViewModel : IDropzoneViewModel
{
	public PanelDropzoneViewModel(
		Key<PanelGroup> panelGroupKey,
		IState<PanelsState> panelsStateWrap,
		IDispatcher dispatcher,
		IDialogService dialogService,
		IJSRuntime jsRuntime,
		MeasuredHtmlElementDimensions measuredHtmlElementDimensions,
		ElementDimensions dropzoneElementDimensions,
		string? cssClassString,
		IPolymorphicViewModel? polymorphicViewModel)
	{
		PanelGroupKey = panelGroupKey;
		PanelsStateWrap = panelsStateWrap;
		Dispatcher = dispatcher;
		DialogService = dialogService;
		JsRuntime = jsRuntime;

		MeasuredHtmlElementDimensions = measuredHtmlElementDimensions;
		DropzoneElementDimensions = dropzoneElementDimensions;

		CssClassString = cssClassString;

		PolymorphicViewModel = polymorphicViewModel;

		Key = Key<IDropzoneViewModel>.NewKey();
	}

	public Key<PanelGroup> PanelGroupKey { get; init; }
	public IState<PanelsState> PanelsStateWrap { get; init; }
	public IDispatcher Dispatcher { get; init; }
	public IDialogService DialogService { get; init; }
	public IJSRuntime JsRuntime { get; init; }

	public IPolymorphicViewModel? PolymorphicViewModel { get; init; }
	public Key<IDropzoneViewModel> Key { get; init; } = Key<IDropzoneViewModel>.NewKey();
	public MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions { get; }
	public ElementDimensions DropzoneElementDimensions { get; }

	public string? CssClassString { get; set; }
}
