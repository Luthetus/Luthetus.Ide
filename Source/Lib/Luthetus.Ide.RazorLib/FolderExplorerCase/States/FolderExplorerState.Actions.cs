using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.RazorLib.FolderExplorerCase.States;

public partial record FolderExplorerState
{
    public record SetFolderExplorerAction(FolderExplorerSync Sync, IAbsolutePath FolderAbsolutePath);

    public record WithAction(Func<FolderExplorerState, FolderExplorerState> WithFunc);
    public record SetFolderExplorerTreeViewAction(FolderExplorerSync Sync, IAbsolutePath FolderAbsolutePath);
}