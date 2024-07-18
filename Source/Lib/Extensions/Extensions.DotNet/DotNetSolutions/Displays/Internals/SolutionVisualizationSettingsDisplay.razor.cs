using Microsoft.AspNetCore.Components;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models.Internals;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Displays.Internals;

public partial class SolutionVisualizationSettingsDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public SolutionVisualizationModel SolutionVisualizationModel { get; set; } = null!;
}