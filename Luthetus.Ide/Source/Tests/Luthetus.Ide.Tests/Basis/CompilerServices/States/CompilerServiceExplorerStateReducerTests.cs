using Fluxor;

namespace Luthetus.Ide.Tests.Basis.CompilerServices.States;

public class CompilerServiceExplorerStateReducerTests
{
    private class Reducer
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