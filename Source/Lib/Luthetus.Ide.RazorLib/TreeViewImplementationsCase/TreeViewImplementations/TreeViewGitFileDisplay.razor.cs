using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.Store.GitCase;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Types.TreeViews;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.TreeViewImplementations;

public partial class TreeViewGitFileDisplay
    : FluxorComponent, ITreeViewGitFileRendererType
{
    [Inject]
    private IState<GitRegistry> GitStateWrap { get; set; } = null!;

    [CascadingParameter]
    public TreeViewState TreeViewState { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeViewGitFile TreeViewGitFile { get; set; } = null!;
}