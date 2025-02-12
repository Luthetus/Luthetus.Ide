using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Extensions.Git.Models;

namespace Luthetus.Extensions.Git.Models;

/// <summary>
/// TODO: Investigate making this a record struct.
/// </summary>
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
    ImmutableList<string> BranchList,
    int? BehindByCommitCount,
    int? AheadByCommitCount)
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
              ImmutableList<string>.Empty,
              null,
              null)
    {

    }
}