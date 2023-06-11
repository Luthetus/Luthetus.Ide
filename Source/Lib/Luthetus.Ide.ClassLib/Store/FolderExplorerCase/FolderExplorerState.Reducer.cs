using Fluxor;

namespace Luthetus.Ide.ClassLib.Store.FolderExplorerCase;

public partial record FolderExplorerState
{
    private class Reducer
    {
        [ReducerMethod]
        public FolderExplorerState ReduceSetFolderExplorerStateAction(
            FolderExplorerState inFolderExplorerState,
            SetFolderExplorerAction setFolderExplorerStateAction)
        {
            return inFolderExplorerState with
            {
                AbsoluteFilePath = setFolderExplorerStateAction.FolderAbsoluteFilePath
            };
        }
    }
}