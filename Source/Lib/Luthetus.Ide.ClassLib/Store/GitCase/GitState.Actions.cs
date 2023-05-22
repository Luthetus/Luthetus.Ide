using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.GitCase;

public partial record GitState
{
    public record SetGitStateWithAction(Func<GitState, GitState> GitStateWithFunc);
    
    public record TryFindGitFolderInDirectoryAction(
        IAbsoluteFilePath DirectoryAbsoluteFilePath,
        CancellationToken CancellationToken);
    
    public record RefreshGitAction(CancellationToken CancellationToken);
    public record GitInitAction(CancellationToken CancellationToken);
}