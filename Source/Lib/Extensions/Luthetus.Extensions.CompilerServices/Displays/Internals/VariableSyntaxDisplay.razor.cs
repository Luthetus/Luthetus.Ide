using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib;

namespace Luthetus.Extensions.CompilerServices.Displays.Internals;

public partial class VariableSyntaxDisplay : ComponentBase
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;
	
	[Parameter, EditorRequired]
	public SyntaxViewModel SyntaxViewModel { get; set; } = default!;
}