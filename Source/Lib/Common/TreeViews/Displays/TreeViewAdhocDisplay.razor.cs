using Luthetus.Common.RazorLib.TreeViews.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.TreeViews.Displays;

public partial class TreeViewAdhocDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewNoType TreeViewNoTypeAdhoc { get; set; } = null!;
}