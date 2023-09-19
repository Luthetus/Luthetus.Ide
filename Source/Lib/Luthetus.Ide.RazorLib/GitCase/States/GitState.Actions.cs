using Luthetus.Common.RazorLib.FileSystem.Models;

namespace Luthetus.Ide.RazorLib.GitCase.States;

public partial record GitState
{
    public record SetGitStateWithAction(Func<GitState, GitState> GitStateWithFunc);

    public record TryFindGitFolderInDirectoryTask(
        GitSync Sync,
        IAbsolutePath DirectoryAbsolutePath,
        CancellationToken CancellationToken);

    public record RefreshGitTask(GitSync Sync, CancellationToken CancellationToken);
    public record GitInitTask(GitSync Sync, CancellationToken CancellationToken);
}