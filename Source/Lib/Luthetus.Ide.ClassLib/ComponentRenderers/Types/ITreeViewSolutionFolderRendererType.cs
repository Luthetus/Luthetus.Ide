using Luthetus.CompilerServices.Lang.DotNetSolution;

namespace Luthetus.Ide.ClassLib.ComponentRenderers.Types;

public interface ITreeViewSolutionFolderRendererType
{
    public DotNetSolutionFolder DotNetSolutionFolder { get; set; }
}