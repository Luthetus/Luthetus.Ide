using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;

namespace Luthetus.Extensions.DotNet.Outputs.Displays;

public partial class OutputPanelDisplay : ComponentBase
{
	[Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;
}