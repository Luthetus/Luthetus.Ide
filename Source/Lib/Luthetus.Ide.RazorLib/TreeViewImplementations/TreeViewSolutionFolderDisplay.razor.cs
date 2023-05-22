using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Luthetus.Ide.ClassLib.DotNet;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations;

public partial class TreeViewSolutionFolderDisplay
    : ComponentBase, ITreeViewSolutionFolderRendererType
{
    [Parameter, EditorRequired]
    public DotNetSolutionFolder DotNetSolutionFolder { get; set; } = null!;
}