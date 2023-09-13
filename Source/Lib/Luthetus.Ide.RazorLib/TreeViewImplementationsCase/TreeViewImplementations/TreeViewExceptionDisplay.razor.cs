using Luthetus.Common.RazorLib.ComponentRenderers.Types.TreeViews;
using Luthetus.Common.RazorLib.WatchWindow.TreeViewClasses;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.TreeViewImplementations;

public partial class TreeViewExceptionDisplay
    : ComponentBase, ITreeViewExceptionRendererType
{
    [Parameter, EditorRequired]
    public TreeViewException TreeViewException { get; set; } = null!;
}