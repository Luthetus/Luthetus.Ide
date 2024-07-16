using Luthetus.CompilerServices.DotNetSolution.Models.Project;

namespace Luthetus.CompilerServices.RazorLib.ComponentRenderers.Models;

public interface ITreeViewSolutionFolderRendererType
{
    public SolutionFolder DotNetSolutionFolder { get; set; }
}