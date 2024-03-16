using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using Fluxor;

namespace Luthetus.TextEditor.RazorLib.PolymorphicUis.Models;

public partial record TextEditorPolymorphicViewModel : IPolymorphicViewModel
{
	public TextEditorPolymorphicViewModel(
		Key<TextEditorViewModel> viewModelKey,
		TextEditorGroup textEditorGroup,
		ITextEditorService textEditorService,
		IDispatcher dispatcher,
		IState<PanelsState> panelsStateWrap,
		IDialogService dialogService,
		IJSRuntime jsRuntime)
	{
		ViewModelKey = viewModelKey;
		TextEditorGroup = textEditorGroup;
		TextEditorService = textEditorService;
		Dispatcher = dispatcher;
		PanelsStateWrap = panelsStateWrap;
		DialogService = dialogService;
		JsRuntime = jsRuntime;

		TabViewModel = new TextEditorTabViewModel(this);
		DraggableViewModel = new TextEditorDraggableViewModel(this);
		DialogViewModel = new TextEditorDialogViewModel(this);

		ActiveViewModelType = typeof(ITabViewModel);
	}

	public Key<TextEditorViewModel> ViewModelKey { get; init; }
	public TextEditorGroup? TextEditorGroup { get; set; }
	public ITextEditorService? TextEditorService { get; init; }
	public IDispatcher Dispatcher { get; init; }
	public IState<PanelsState> PanelsStateWrap { get; init; }
	public IJSRuntime? JsRuntime { get; init; }
	public IDialogService DialogService { get; init; }

	public Type? ActiveViewModelType { get; set; }
	public IDialogViewModel? DialogViewModel { get; }
    public IDraggableViewModel? DraggableViewModel { get; }
    public IDropzoneViewModel? DropzoneViewModel { get; }
    public INotificationViewModel? NotificationViewModel { get; }
    public ITabViewModel? TabViewModel { get; }

	public TextEditorViewModelDisplayOptions TextEditorViewModelDisplayOptions { get; } = new()
	{
		IncludeHeaderHelperComponent = false,
	};

	public string GetTitle()
	{
		if (TextEditorService is null)
			return "TextEditorService was null";

		var model = TextEditorService.ViewModelApi.GetModelOrDefault(ViewModelKey);
		var viewModel = TextEditorService.ViewModelApi.GetOrDefault(ViewModelKey);

		if (viewModel is null)
        {
            return "ViewModel not found";
        }
		else if (model is null)
		{
            return "Model not found";
		}
		else
		{
			var displayName = viewModel.GetTabDisplayNameFunc?.Invoke(model)
				?? model.ResourceUri.Value;

			if (model.IsDirty)
				displayName += '*';

			return displayName;
		}
	}
}