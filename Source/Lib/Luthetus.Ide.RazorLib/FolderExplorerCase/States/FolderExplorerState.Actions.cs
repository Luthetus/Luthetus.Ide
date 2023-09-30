using Luthetus.Common.RazorLib.FileSystem.Models;

namespace Luthetus.Ide.RazorLib.FolderExplorerCase.States;

public partial record FolderExplorerState
{
    public record WithAction(Func<FolderExplorerState, FolderExplorerState> WithFunc);
}