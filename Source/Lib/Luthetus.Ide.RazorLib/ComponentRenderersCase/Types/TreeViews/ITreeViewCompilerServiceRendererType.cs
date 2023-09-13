using Luthetus.Common.RazorLib.WatchWindow.TreeViewClasses;
using Luthetus.Ide.ClassLib.TreeViewImplementationsCase.CompilerServiceExplorerCase;

namespace Luthetus.Ide.ClassLib.ComponentRenderers.Types.TreeViews;

public interface ITreeViewCompilerServiceRendererType
{
    public TreeViewCompilerService TreeViewCompilerService { get; set; }
}
