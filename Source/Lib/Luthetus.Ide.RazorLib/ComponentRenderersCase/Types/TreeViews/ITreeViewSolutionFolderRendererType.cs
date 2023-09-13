using Luthetus.CompilerServices.Lang.DotNetSolution;

namespace Luthetus.Ide.RazorLib.ComponentRenderersCase.Types.TreeViews;

public interface ITreeViewSolutionFolderRendererType
{
    public DotNetSolutionFolder DotNetSolutionFolder { get; set; }
}