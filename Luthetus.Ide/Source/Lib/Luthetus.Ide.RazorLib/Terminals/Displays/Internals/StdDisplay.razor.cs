using Luthetus.Ide.RazorLib.Terminals.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Terminals.Displays.Internals;

public partial class StdDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public Std Std { get; set; } = null!;
}