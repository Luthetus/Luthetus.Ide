using Fluxor;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Displays;

public partial class TreeViewGitFileGroupDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public GitState GitState { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeViewGitFileGroup TreeViewGitFileGroup { get; set; } = null!;
}