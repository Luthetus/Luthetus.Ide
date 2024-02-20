using Fluxor;

namespace Luthetus.Ide.Tests.Basis.FolderExplorers.States;

public class FolderExplorerStateReducerTests
{
    private class Reducer
    {
        [ReducerMethod]
        public static FolderExplorerState ReduceWithAction(
            FolderExplorerState inState,
            WithAction withAction)
        {
            return withAction.WithFunc.Invoke(inState);
        }
    }
}