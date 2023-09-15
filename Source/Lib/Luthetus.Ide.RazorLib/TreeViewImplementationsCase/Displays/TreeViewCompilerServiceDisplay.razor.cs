using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Displays;

public partial class TreeViewCompilerServiceDisplay : ComponentBase,
    ITreeViewCompilerServiceRendererType
{
    [Parameter, EditorRequired]
    public TreeViewCompilerService TreeViewCompilerService { get; set; } = null!;
}