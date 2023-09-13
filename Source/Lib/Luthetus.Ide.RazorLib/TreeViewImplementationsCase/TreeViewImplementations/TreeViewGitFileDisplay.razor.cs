using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types.TreeViews;
using Luthetus.Ide.ClassLib.Store.GitCase;
using Luthetus.Ide.ClassLib.TreeViewImplementationsCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations;

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