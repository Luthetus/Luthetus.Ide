using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Luthetus.Ide.RazorLib.GitCase.States;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Displays;

public partial class TreeViewGitFileDisplay
    : FluxorComponent, ITreeViewGitFileRendererType
{
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;

    [CascadingParameter]
    public TreeViewState TreeViewState { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeViewGitFile TreeViewGitFile { get; set; } = null!;
}