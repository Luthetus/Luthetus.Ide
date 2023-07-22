using Luthetus.CompilerServices.Lang.DotNet;

namespace Luthetus.Ide.ClassLib.ComponentRenderers.Types;

public interface ITreeViewSolutionFolderRendererType
{
    public DotNetSolutionFolder DotNetSolutionFolder { get; set; }
}