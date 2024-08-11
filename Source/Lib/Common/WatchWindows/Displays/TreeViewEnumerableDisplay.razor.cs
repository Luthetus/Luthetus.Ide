using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.Options.States;

namespace Luthetus.Common.RazorLib.WatchWindows.Displays;

public partial class TreeViewEnumerableDisplay : ComponentBase
{
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TreeViewEnumerable TreeViewEnumerable { get; set; } = null!;
}