using System.Collections.Immutable;
using Luthetus.Extensions.Git.Models;

namespace Luthetus.Extensions.Git.States;

public partial record GitState
{
    public record SetRepoAction(GitRepo? Repo);

    /// <summary>
    /// If the expected path is not the actual path, then the git file list will NOT be changed.
    /// </summary>
    public record SetStatusAction(
        GitRepo Repo,
        ImmutableList<GitFile> UntrackedFileList,
        ImmutableList<GitFile> StagedFileList,
        ImmutableList<GitFile> UnstagedFileList,
        int? BehindByCommitCount,
        int? AheadByCommitCount);

    public record SetOriginAction(GitRepo Repo, string Origin);
    public record SetBranchAction(GitRepo Repo, string Branch);
    public record SetBranchListAction(GitRepo Repo, List<string> BranchList);

    public record SetSelectedFileListAction(
        Func<ImmutableDictionary<string, GitFile>,
            ImmutableDictionary<string, GitFile>> SetSelectedFileListFunc);

    public record RefreshAction;
    public record WithAction(Func<GitState, GitState> WithFunc);
}