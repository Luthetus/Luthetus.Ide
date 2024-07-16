using Microsoft.AspNetCore.Components;

namespace Luthetus.CompilerServices.RazorLib.TestExplorers.Displays.Internals;

public partial class TestExplorerContextMenu : ComponentBase
{
	[Inject]
    private DotNetCliOutputParser DotNetCliOutputParser { get; set; } = null!;
}