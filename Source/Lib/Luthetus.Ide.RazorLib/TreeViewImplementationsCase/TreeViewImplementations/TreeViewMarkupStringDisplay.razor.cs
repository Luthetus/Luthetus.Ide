using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations;

public partial class TreeViewMarkupStringDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public MarkupString MarkupString { get; set; }
}