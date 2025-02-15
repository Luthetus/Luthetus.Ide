using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Extensions.Git.Models;

/// <summary>
/// TODO: Investigate making this a record struct.
/// </summary>
public partial record GitState(
    GitRepo? Repo,
    List<GitFile> UntrackedFileList,
    List<GitFile> StagedFileList,
    List<GitFile> UnstagedFileList,
    List<GitFile> SelectedFileList,
    List<GitTask> ActiveTasks,
    string? Origin,
    string? Branch,
    string? Upstream,
	List<string> BranchList,
    int? BehindByCommitCount,
    int? AheadByCommitCount)
{
    public static readonly Key<TreeViewContainer> TreeViewGitChangesKey = Key<TreeViewContainer>.NewKey();

    public GitState()
        : this(
              null,
              new(),
              new(),
              new(),
              new(),
              new(),
              null,
              null,
              null,
              new(),
              null,
              null)
    {

    }
}