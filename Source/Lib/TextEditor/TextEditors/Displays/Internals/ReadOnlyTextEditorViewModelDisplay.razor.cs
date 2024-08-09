using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.ReadOnlys;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class ReadOnlyTextEditorViewModelDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public Key<TextEditorViewModel> TextEditorViewModelKey { get; set; } = Key<TextEditorViewModel>.Empty;

	private ViewModelDisplayOptions _textEditorViewModelDisplayOptions = new()
	{
		IncludeHeaderHelperComponent = false,
		IncludeFooterHelperComponent = false,
		IncludeGutterComponent = false,
		ContextRecord = ContextFacts.TerminalContext,
	};

	private Key<TextEditorViewModel> _seenTextEditorViewModelKey;

	protected override void OnParametersSet()
	{
		if (_seenTextEditorViewModelKey != TextEditorViewModelKey)
		{
			_seenTextEditorViewModelKey = TextEditorViewModelKey;

			_textEditorViewModelDisplayOptions = new()
			{
				IncludeHeaderHelperComponent = false,
				IncludeFooterHelperComponent = false,
				IncludeGutterComponent = false,
				ContextRecord = ContextFacts.TerminalContext,
				KeymapOverride = new TextEditorKeymapReadOnly(() => TextEditorViewModelKey)
			};
		}

		base.OnParametersSet();
	}
}