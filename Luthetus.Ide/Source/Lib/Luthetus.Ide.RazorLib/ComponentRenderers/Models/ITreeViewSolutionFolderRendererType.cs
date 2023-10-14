using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;

namespace Luthetus.Ide.RazorLib.ComponentRenderers.Models;

public interface ITreeViewSolutionFolderRendererType
{
    public SolutionFolder DotNetSolutionFolder { get; set; }
}