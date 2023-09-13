using Luthetus.Ide.RazorLib.ComponentRenderersCase.Types.TreeViews;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.CompilerServiceExplorerCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.TreeViewImplementations;

public partial class TreeViewCompilerServiceDisplay : ComponentBase,
    ITreeViewCompilerServiceRendererType
{
    [Parameter, EditorRequired]
    public TreeViewCompilerService TreeViewCompilerService { get; set; } = null!;
}