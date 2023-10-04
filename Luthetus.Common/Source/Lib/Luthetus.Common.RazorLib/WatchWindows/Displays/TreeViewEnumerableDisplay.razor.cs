using Luthetus.Common.RazorLib.WatchWindows.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.WatchWindows.Displays;

public partial class TreeViewEnumerableDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewEnumerable TreeViewEnumerable { get; set; } = null!;
}