using Luthetus.CompilerServices.Lang.DotNetSolution;

namespace Luthetus.Ide.ClassLib.ComponentRenderers.Types.TreeViews;

public interface ITreeViewSolutionFolderRendererType
{
    public DotNetSolutionFolder DotNetSolutionFolder { get; set; }
}