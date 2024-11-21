using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.Symbols;

public partial class TypeSyntaxDisplay : ComponentBase
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;
	
	[Parameter, EditorRequired]
	public SyntaxViewModel SyntaxViewModel { get; set; } = default!;

	[Parameter, EditorRequired]
	public (string ClassCssString, string Text, TextEditorTextSpan TextSpan) Parameter { get; set; }
	
	private Task HandleOnClick(SyntaxViewModel syntaxViewModelLocal)
	{
		return TextEditorService.OpenInEditorAsync(
			Parameter.TextSpan.ResourceUri.Value,
			true,
			Parameter.TextSpan.StartingIndexInclusive,
			new Category("main"),
			Key<TextEditorViewModel>.NewKey());
	}
}