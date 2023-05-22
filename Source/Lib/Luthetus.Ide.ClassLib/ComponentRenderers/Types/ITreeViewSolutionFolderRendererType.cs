using Luthetus.Ide.ClassLib.DotNet;

namespace Luthetus.Ide.ClassLib.ComponentRenderers.Types;

public interface ITreeViewSolutionFolderRendererType
{
    public DotNetSolutionFolder DotNetSolutionFolder { get; set; }
}