using Luthetus.CompilerServices.Lang.DotNetSolution;

namespace Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;

public interface ITreeViewSolutionFolderRendererType
{
    public DotNetSolutionFolder DotNetSolutionFolder { get; set; }
}