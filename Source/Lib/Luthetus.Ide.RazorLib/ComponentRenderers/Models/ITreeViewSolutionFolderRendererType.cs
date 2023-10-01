using Luthetus.CompilerServices.Lang.DotNetSolution;

namespace Luthetus.Ide.RazorLib.ComponentRenderers.Models;

public interface ITreeViewSolutionFolderRendererType
{
    public DotNetSolutionFolder DotNetSolutionFolder { get; set; }
}