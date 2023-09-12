using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.GitCase;

public partial record GitRegistry
{
    public record SetGitStateWithAction(Func<GitRegistry, GitRegistry> GitStateWithFunc);

    public record TryFindGitFolderInDirectoryAction(
        IAbsolutePath DirectoryAbsoluteFilePath,
        CancellationToken CancellationToken);

    public record RefreshGitAction(CancellationToken CancellationToken);
    public record GitInitAction(CancellationToken CancellationToken);
}