using Microsoft.AspNetCore.Components;
using Luthetus.Extensions.CompilerServices.Syntax;

namespace Luthetus.Extensions.CompilerServices.Displays;

public partial class SymbolDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public Symbol Symbol { get; set; } = default!;
}