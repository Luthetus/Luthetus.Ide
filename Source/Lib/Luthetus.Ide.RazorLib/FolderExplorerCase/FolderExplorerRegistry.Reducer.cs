using Fluxor;

namespace Luthetus.Ide.RazorLib.FolderExplorerCase;

public partial record FolderExplorerRegistry
{
    private class Reducer
    {
        [ReducerMethod]
        public FolderExplorerRegistry ReduceSetFolderExplorerStateAction(
            FolderExplorerRegistry inFolderExplorerState,
            SetFolderExplorerAction setFolderExplorerStateAction)
        {
            return inFolderExplorerState with
            {
                AbsolutePath = setFolderExplorerStateAction.FolderAbsolutePath
            };
        }
    }
}