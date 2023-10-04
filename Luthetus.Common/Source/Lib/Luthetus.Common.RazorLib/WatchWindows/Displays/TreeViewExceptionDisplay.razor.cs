using Luthetus.Common.RazorLib.WatchWindows.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.WatchWindows.Displays;

public partial class TreeViewExceptionDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewException TreeViewException { get; set; } = null!;
}