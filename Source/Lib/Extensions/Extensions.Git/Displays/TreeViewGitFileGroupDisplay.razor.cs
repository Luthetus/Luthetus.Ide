using Microsoft.AspNetCore.Components;
using Luthetus.Extensions.Git.Models;

namespace Luthetus.Extensions.Git.Displays;

public partial class TreeViewGitFileGroupDisplay : ComponentBase
{
    [CascadingParameter]
    public GitState GitState { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeViewGitFileGroup TreeViewGitFileGroup { get; set; } = null!;
}