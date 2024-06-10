using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.CommandLines.Models;

namespace Luthetus.Ide.RazorLib.Outputs.Displays.Internals;

public partial class OutputDisplay : ComponentBase
{
	[Inject]
	private DotNetCliOutputParser DotNetCliOutputParser { get; set; } = null!;
}