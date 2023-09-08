using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Common.RazorLib.WatchWindow.TreeViewClasses;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types.TreeViews;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations;

public partial class TreeViewCompilerServiceDisplay
    : ComponentBase, ITreeViewCompilerServiceRendererType
{
    [Parameter, EditorRequired]
    public TreeViewCompilerService TreeViewCompilerService { get; set; } = null!;
}