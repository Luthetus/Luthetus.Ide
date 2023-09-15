using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Displays;

public partial class TreeViewMarkupStringDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public MarkupString MarkupString { get; set; }
}