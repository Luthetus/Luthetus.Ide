using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewUtils.Displays;

public partial class TreeViewMarkupStringDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public MarkupString MarkupString { get; set; }
}