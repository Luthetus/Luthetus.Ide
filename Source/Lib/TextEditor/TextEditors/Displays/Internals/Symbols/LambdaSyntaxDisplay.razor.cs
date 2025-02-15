using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.Symbols;

public partial class LambdaSyntaxDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public SyntaxViewModel SyntaxViewModel { get; set; } = default!;
}
