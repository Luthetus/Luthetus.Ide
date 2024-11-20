using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class SymbolOnClick : ComponentBase
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;

	[Parameter, EditorRequired]
	public (string ClassCssString, string Text, TextEditorTextSpan TextSpan) Parameter  { get; set; }
	
	private Task HandleOnClick()
	{
		return TextEditorService.OpenInEditorAsync(
			Parameter.TextSpan.ResourceUri.Value,
			true,
			Parameter.TextSpan.StartingIndexInclusive,
			new Category("main"),
			Key<TextEditorViewModel>.NewKey());
	}
}