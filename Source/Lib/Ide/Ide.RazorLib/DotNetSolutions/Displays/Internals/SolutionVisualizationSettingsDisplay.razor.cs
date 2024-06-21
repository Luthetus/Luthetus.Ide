using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.DotNetSolutions.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Displays.Internals;

public partial class SolutionVisualizationSettingsDisplay : ComponentBase
{
	[Parameter, EditorRequired]
    public SolutionVisualizationModel SolutionVisualizationModel { get; set; } = null!;
}