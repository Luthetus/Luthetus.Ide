using Microsoft.AspNetCore.Components;
using Luthetus.CompilerServices.RazorLib.DotNetSolutions.Models.Internals;

namespace Luthetus.CompilerServices.RazorLib.DotNetSolutions.Displays.Internals;

public partial class SolutionVisualizationSettingsDisplay : ComponentBase
{
	[Parameter, EditorRequired]
    public SolutionVisualizationModel SolutionVisualizationModel { get; set; } = null!;
}