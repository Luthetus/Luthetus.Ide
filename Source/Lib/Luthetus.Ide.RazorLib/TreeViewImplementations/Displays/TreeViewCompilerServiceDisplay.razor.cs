using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Displays;

public partial class TreeViewCompilerServiceDisplay : ComponentBase,
    ITreeViewCompilerServiceRendererType
{
    [Parameter, EditorRequired]
    public TreeViewCompilerService TreeViewCompilerService { get; set; } = null!;
}