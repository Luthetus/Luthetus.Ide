using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.RazorLib.GitCase.States;

public partial record GitRegistry
{
    public record SetGitStateWithAction(Func<GitRegistry, GitRegistry> GitStateWithFunc);

    public record TryFindGitFolderInDirectoryAction(
        IAbsolutePath DirectoryAbsolutePath,
        CancellationToken CancellationToken);

    public record RefreshGitAction(CancellationToken CancellationToken);
    public record GitInitAction(CancellationToken CancellationToken);
}