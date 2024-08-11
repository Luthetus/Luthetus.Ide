using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Displays;

public partial class TreeViewSolutionFolderDisplay : ComponentBase, ITreeViewSolutionFolderRendererType
{
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    
	[Parameter, EditorRequired]
	public SolutionFolder DotNetSolutionFolder { get; set; } = null!;
}