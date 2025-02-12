using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Displays;

public partial class TreeViewSolutionFolderDisplay : ComponentBase, ITreeViewSolutionFolderRendererType
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    
	[Parameter, EditorRequired]
	public SolutionFolder DotNetSolutionFolder { get; set; } = null!;
}