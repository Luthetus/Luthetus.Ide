using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class SymbolOnClick : ComponentBase
{
	[Parameter, EditorRequired]
	public (string ClassCssString, string Text, TextEditorTextSpan TextSpan) Parameter  { get; set; }
	
	private void HandleOnClick()
	{
		
	}
}