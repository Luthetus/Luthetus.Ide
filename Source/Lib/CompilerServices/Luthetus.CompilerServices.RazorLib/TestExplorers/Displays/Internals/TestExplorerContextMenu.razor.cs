using Microsoft.AspNetCore.Components;
using Luthetus.CompilerServices.RazorLib.CommandLines.Models;

namespace Luthetus.CompilerServices.RazorLib.TestExplorers.Displays.Internals;

public partial class TestExplorerContextMenu : ComponentBase
{
	[Inject]
    private DotNetCliOutputParser DotNetCliOutputParser { get; set; } = null!;
}