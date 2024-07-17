using Microsoft.AspNetCore.Components;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Displays;

public partial class TreeViewSolutionFolderDisplay : ComponentBase, ITreeViewSolutionFolderRendererType
{
	[Parameter, EditorRequired]
	public SolutionFolder DotNetSolutionFolder { get; set; } = null!;
}