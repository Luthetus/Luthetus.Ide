using Luthetus.CompilerServices.Lang.DotNetSolution;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Displays;

public partial class TreeViewSolutionFolderDisplay
    : ComponentBase, ITreeViewSolutionFolderRendererType
{
    [Parameter, EditorRequired]
    public DotNetSolutionFolder DotNetSolutionFolder { get; set; } = null!;
}