using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.PolymorphicUis.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.Common.RazorLib.PolymorphicUis.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.PolymorphicUis.Models;

public partial record TextEditorViewModelPolymorphicUi : IPolymorphicTab
{
	public bool TabIsActive => TextEditorGroup.ActiveViewModelKey == ViewModelKey;

	public Dictionary<string, object?>? TabParameterMap => new Dictionary<string, object?>
	{
		{
			nameof(PolymorphicTabDisplay.Tab),
			this
		},
		{
			nameof(PolymorphicTabDisplay.IsBeingDragged),
			true
		}
	};

	public Task TabOnClickAsync(MouseEventArgs mouseEventArgs)
	{
		if (!TabIsActive)
			TextEditorService.GroupApi.SetActiveViewModel(TextEditorGroup.GroupKey, ViewModelKey);
		
		return Task.CompletedTask;
	}

	public string TabGetDynamicCss()
	{
		return string.Empty;
	}

	public Task TabCloseAsync()
	{
		TextEditorService.GroupApi.RemoveViewModel(TextEditorGroup.GroupKey, ViewModelKey);
		return Task.CompletedTask;
	}
}
