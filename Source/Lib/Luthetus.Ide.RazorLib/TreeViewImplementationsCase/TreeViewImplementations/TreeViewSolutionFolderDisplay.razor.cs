using Luthetus.CompilerServices.Lang.DotNetSolution;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Types.TreeViews;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.TreeViewImplementations;

public partial class TreeViewSolutionFolderDisplay
    : ComponentBase, ITreeViewSolutionFolderRendererType
{
    [Parameter, EditorRequired]
    public DotNetSolutionFolder DotNetSolutionFolder { get; set; } = null!;
}