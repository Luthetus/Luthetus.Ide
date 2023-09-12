using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.FolderExplorerCase;

public partial record FolderExplorerRegistry
{
    public record SetFolderExplorerAction(IAbsolutePath FolderAbsoluteFilePath);

    private record WithAction(Func<FolderExplorerRegistry, FolderExplorerRegistry> WithFunc);
    private record SetFolderExplorerTreeViewAction;
}