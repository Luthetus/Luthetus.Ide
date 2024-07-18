using Microsoft.AspNetCore.Components;
using Luthetus.Extensions.DotNet.CommandLines.Models;

namespace Luthetus.Extensions.DotNet.StartupControls.Displays;

public partial class StartupControlsDisplay : ComponentBase
{
	[Inject]
	private DotNetCliOutputParser DotNetCliOutputParser { get; set; } = null!;
}