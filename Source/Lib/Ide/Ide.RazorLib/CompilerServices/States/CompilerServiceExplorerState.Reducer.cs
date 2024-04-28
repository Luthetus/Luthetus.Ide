using Fluxor;

namespace Luthetus.Ide.RazorLib.CompilerServices.States;

public partial class CompilerServiceExplorerState
{
    public class Reducer
    {
        [ReducerMethod]
        public CompilerServiceExplorerState ReduceNewAction(
            CompilerServiceExplorerState inCompilerServiceExplorerState,
            NewAction newAction)
        {
            return newAction.NewFunc.Invoke(inCompilerServiceExplorerState);
        }
    }
}