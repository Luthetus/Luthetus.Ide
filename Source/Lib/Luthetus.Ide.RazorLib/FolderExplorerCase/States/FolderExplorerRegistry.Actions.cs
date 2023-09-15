using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.RazorLib.FolderExplorerCase.States;

public partial record FolderExplorerRegistry
{
    public record SetFolderExplorerAction(IAbsolutePath FolderAbsolutePath);

    private record WithAction(Func<FolderExplorerRegistry, FolderExplorerRegistry> WithFunc);
    private record SetFolderExplorerTreeViewAction;
}