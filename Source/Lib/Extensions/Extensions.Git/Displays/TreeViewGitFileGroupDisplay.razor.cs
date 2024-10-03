using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Extensions.Git.Models;
using Luthetus.Extensions.Git.States;

namespace Luthetus.Extensions.Git.Displays;

public partial class TreeViewGitFileGroupDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public GitState GitState { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeViewGitFileGroup TreeViewGitFileGroup { get; set; } = null!;
}