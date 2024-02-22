using Fluxor;

namespace Luthetus.Ide.RazorLib.FolderExplorers.States;

public partial record FolderExplorerState
{
    public class Reducer
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