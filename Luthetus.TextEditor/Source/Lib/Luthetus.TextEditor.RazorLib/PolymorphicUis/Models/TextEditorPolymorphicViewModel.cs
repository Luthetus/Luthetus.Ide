using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
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

namespace Luthetus.TextEditor.RazorLib.PolymorphicUis.Models;

public partial record TextEditorPolymorphicViewModel : IPolymorphicViewModel
{
	public TextEditorPolymorphicViewModel(
		Key<TextEditorViewModel> viewModelKey,
		TextEditorGroup textEditorGroup,
		ITextEditorService textEditorService,
		IDialogService dialogService,
		IJSRuntime jsRuntime)
	{
		ViewModelKey = viewModelKey;
		TextEditorGroup = textEditorGroup;
		TextEditorService = textEditorService;
		DialogService = dialogService;
		JsRuntime = jsRuntime;
	}

	/// <summary>
	/// TODO: Aquire access to the TextEditorService via the constructor...
	/// ...This hack was used instead, because adding the TextEditorService
	/// to the view model registration logic seems like an odd thing to do.
	/// This needs to be looked into more.
	///
	/// The hack is that: the UI when it sees an instance of this type,
	/// it will set this type's TextEditorService property.
	/// </summary>
	public ITextEditorService? TextEditorService { get; set; }
	public IJSRuntime? JsRuntime { get; set; }
	public IDialogService DialogService { get; set; }

	public Key<TextEditorViewModel> ViewModelKey { get; }
	public TextEditorGroup TextEditorGroup { get; set; }

	public IDialogViewModel? DialogViewModel { get; }
    public IDraggableViewModel? DraggableViewModel { get; }
    public IDropzoneViewModel? DropzoneViewModel { get; }
    public INotificationViewModel? NotificationViewModel { get; }
    public ITabViewModel? TabViewModel { get; }

	private readonly TextEditorViewModelDisplayOptions _textEditorViewModelDisplayOptions = new()
	{
		IncludeHeaderHelperComponent = false,
	};

	private string GetTitle()
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