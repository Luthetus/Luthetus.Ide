using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.Options.Models;


namespace Luthetus.Common.RazorLib.TreeViews.Displays.Utils;

public partial class TreeViewExceptionDisplay : ComponentBase, ITreeViewExceptionRendererType
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TreeViewException TreeViewException { get; set; } = null!;
}