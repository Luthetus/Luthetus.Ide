using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.Options.Models;

namespace Luthetus.Common.RazorLib.WatchWindows.Displays;

public partial class TreeViewPropertiesDisplay : ComponentBase
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TreeViewProperties TreeViewProperties { get; set; } = null!;
}