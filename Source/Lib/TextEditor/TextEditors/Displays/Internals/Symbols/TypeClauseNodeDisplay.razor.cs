using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.Symbols;

public partial class TypeClauseNodeDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public SymbolViewModel SymbolViewModel { get; set; } = default!;
}