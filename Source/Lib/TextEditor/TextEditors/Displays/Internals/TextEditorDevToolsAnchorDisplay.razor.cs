using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextEditorDevToolsAnchorDisplay : ComponentBase, ITextEditorDependentComponent
{
	[Parameter, EditorRequired]
	public TextEditorViewModelDisplay TextEditorViewModelDisplay { get; set; } = null!;
	
	public void Dispose()
	{
		return;
	}
}