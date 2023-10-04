using Luthetus.Common.RazorLib.WatchWindows.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.WatchWindows.Displays;

public partial class TreeViewTextDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewText TreeViewText { get; set; } = null!;
}