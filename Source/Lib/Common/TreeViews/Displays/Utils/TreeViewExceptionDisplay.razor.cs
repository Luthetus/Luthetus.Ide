using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;


namespace Luthetus.Common.RazorLib.TreeViews.Displays.Utils;

public partial class TreeViewExceptionDisplay : ComponentBase, ITreeViewExceptionRendererType
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TreeViewException TreeViewException { get; set; } = null!;
}