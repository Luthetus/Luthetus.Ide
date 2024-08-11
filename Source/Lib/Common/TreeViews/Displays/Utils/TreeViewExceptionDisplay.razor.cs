using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.Options.States;


namespace Luthetus.Common.RazorLib.TreeViews.Displays.Utils;

public partial class TreeViewExceptionDisplay : ComponentBase, ITreeViewExceptionRendererType
{
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TreeViewException TreeViewException { get; set; } = null!;
}