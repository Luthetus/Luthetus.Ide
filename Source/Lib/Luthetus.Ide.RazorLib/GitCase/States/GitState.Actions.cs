using Luthetus.Common.RazorLib.FileSystem.Models;

namespace Luthetus.Ide.RazorLib.GitCase.States;

public partial record GitState
{
    public record SetGitStateWithAction(Func<GitState, GitState> GitStateWithFunc);

    public record TryFindGitFolderInDirectoryAction(
        IAbsolutePath DirectoryAbsolutePath,
        CancellationToken CancellationToken);

    public record RefreshGitAction(CancellationToken CancellationToken);
    public record GitInitAction(CancellationToken CancellationToken);
}