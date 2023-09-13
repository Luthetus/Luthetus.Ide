using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.TreeViewImplementations;

public partial class TreeViewMarkupStringDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public MarkupString MarkupString { get; set; }
}