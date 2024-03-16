using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.PolymorphicUis.Models;

public record TextEditorDialogViewModel : IDialogViewModel
{
	private readonly Func<string> _getTitleFunc;

	public TextEditorDialogViewModel(
		Key<TextEditorViewModel> viewModelKey,
		TextEditorViewModelDisplayOptions textEditorViewModelDisplayOptions,
		Func<string> getTitleFunc,
		IPolymorphicViewModel? polymorphicViewModel)
	{
		ViewModelKey = viewModelKey;
		TextEditorViewModelDisplayOptions = textEditorViewModelDisplayOptions;
		_getTitleFunc = getTitleFunc;
		PolymorphicViewModel = polymorphicViewModel;

		Key = new(ViewModelKey.Guid);

		IsResizable = true;

		RendererType = typeof(TextEditorViewModelDisplay);

		ParameterMap = new()
		{
			{
				nameof(TextEditorViewModelDisplay.TextEditorViewModelKey),
				ViewModelKey
			},
			{
				nameof(TextEditorViewModelDisplay.ViewModelDisplayOptions),
				TextEditorViewModelDisplayOptions
			}
		};
	}

	public Key<TextEditorViewModel> ViewModelKey { get; init; }
	public TextEditorViewModelDisplayOptions TextEditorViewModelDisplayOptions { get; init; }
	public IPolymorphicViewModel? PolymorphicViewModel { get; init; }
	public Key<IDialogViewModel> Key { get; init; }
	public Type RendererType { get; init; }
	public string Title => _getTitleFunc.Invoke();
	public Dictionary<string, object?>? ParameterMap { get; init; }
	public ElementDimensions ElementDimensions { get; init; } = IDialogViewModel.ConstructDefaultElementDimensions();
    public bool IsMinimized { get; init; }
    public bool IsMaximized { get; init; }
    public bool IsResizable { get; init; }
    public string CssClassString { get; init; }
    public string FocusPointHtmlElementId => $"luth_dialog-focus-point_{Key.Guid}";

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
