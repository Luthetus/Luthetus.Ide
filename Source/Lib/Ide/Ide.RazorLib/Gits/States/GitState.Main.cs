using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Gits.States;

[FeatureState]
public partial record GitState(
    GitRepo? Repo,
    ImmutableList<GitFile> UntrackedFileList,
    ImmutableList<GitFile> StagedFileList,
    ImmutableList<GitFile> UnstagedFileList,
    ImmutableList<GitFile> SelectedFileList,
    ImmutableList<GitTask> ActiveTasks,
    string? Origin,
    string? Branch,
    string? Upstream,
    ImmutableList<string> BranchList)
{
    public static readonly Key<TreeViewContainer> TreeViewGitChangesKey = Key<TreeViewContainer>.NewKey();

    public GitState()
        : this(
              null,
              ImmutableList<GitFile>.Empty,
              ImmutableList<GitFile>.Empty,
              ImmutableList<GitFile>.Empty,
              ImmutableList<GitFile>.Empty,
              ImmutableList<GitTask>.Empty,
              null,
              null,
              null,
              ImmutableList<string>.Empty)
    {

    }
}