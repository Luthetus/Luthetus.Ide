using Microsoft.AspNetCore.Components;
using Luthetus.CompilerServices.RazorLib.CommandLines.Models;

namespace Luthetus.CompilerServices.RazorLib.StartupControls.Displays;

public partial class StartupControlsDisplay : ComponentBase
{
    [Inject]
    private DotNetCliOutputParser DotNetCliOutputParser { get; set; } = null!;
}