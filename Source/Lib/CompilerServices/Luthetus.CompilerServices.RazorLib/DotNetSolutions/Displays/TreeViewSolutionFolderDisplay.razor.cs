using Microsoft.AspNetCore.Components;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.CompilerServices.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

namespace Luthetus.CompilerServices.RazorLib.DotNetSolutions.Displays;

public partial class TreeViewSolutionFolderDisplay : ComponentBase, ITreeViewSolutionFolderRendererType
{
    [Parameter, EditorRequired]
    public SolutionFolder DotNetSolutionFolder { get; set; } = null!;
}