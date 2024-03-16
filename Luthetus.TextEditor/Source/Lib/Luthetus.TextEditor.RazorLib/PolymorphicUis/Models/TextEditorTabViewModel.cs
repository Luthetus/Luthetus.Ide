using Luthetus.Common.RazorLib.Tabs.Displays;
using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.PolymorphicUis.Models;

public partial record TextEditorTabViewModel : ITabViewModel
{
	public TextEditorTabViewModel(TextEditorPolymorphicViewModel textEditorPolymorphicViewModel)
	{
		TextEditorPolymorphicViewModel = textEditorPolymorphicViewModel;
		PolymorphicViewModel = textEditorPolymorphicViewModel;

		Key = new(TextEditorPolymorphicViewModel.ViewModelKey.Guid);
	}

	public TextEditorPolymorphicViewModel TextEditorPolymorphicViewModel { get; init; }
	public IPolymorphicViewModel? PolymorphicViewModel { get; }
	public Key<ITabViewModel> Key { get; }
	public string Title => TextEditorPolymorphicViewModel.GetTitle();

	public Dictionary<string, object?>? ParameterMap { get; }

	public bool GetIsActive()
	{
		return TextEditorPolymorphicViewModel.TextEditorGroup.ActiveViewModelKey == TextEditorPolymorphicViewModel.ViewModelKey;
	}

	public Task OnClickAsync(MouseEventArgs mouseEventArgs)
	{
		if (!GetIsActive())
			TextEditorPolymorphicViewModel.TextEditorService.GroupApi.SetActiveViewModel(TextEditorPolymorphicViewModel.TextEditorGroup.GroupKey, TextEditorPolymorphicViewModel.ViewModelKey);
		
		return Task.CompletedTask;
	}

	public string GetDynamicCss()
	{
		return string.Empty;
	}

	public Task CloseAsync()
	{
		TextEditorPolymorphicViewModel.TextEditorService.GroupApi.RemoveViewModel(TextEditorPolymorphicViewModel.TextEditorGroup.GroupKey, TextEditorPolymorphicViewModel.ViewModelKey);
		return Task.CompletedTask;
	}
}
