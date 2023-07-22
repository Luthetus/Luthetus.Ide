using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.FolderExplorerCase;

public partial record FolderExplorerState
{
    public record SetFolderExplorerAction(IAbsoluteFilePath FolderAbsoluteFilePath);

    private record WithAction(Func<FolderExplorerState, FolderExplorerState> WithFunc);
    private record SetFolderExplorerTreeViewAction;
}