using Microsoft.AspNetCore.Components;

namespace Luthetus.CompilerServices.RazorLib.StartupControls.Displays;

public partial class StartupControlsDisplay : ComponentBase
{
    [Inject]
    private DotNetCliOutputParser DotNetCliOutputParser { get; set; } = null!;
}