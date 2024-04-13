using Luthetus.Common.RazorLib.WatchWindows.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.WatchWindows.Displays;

public partial class TreeViewFieldsDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewFields TreeViewFields { get; set; } = null!;
}