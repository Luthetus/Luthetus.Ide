using Fluxor;

namespace Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase;

public partial record CompilerServiceExplorerState
{
    private class Reducer
    {
        [ReducerMethod]
        public CompilerServiceExplorerState ReduceSetCompilerServiceExplorerStateAction(
            CompilerServiceExplorerState inCompilerServiceExplorerState,
            SetCompilerServiceExplorerAction setCompilerServiceExplorerStateAction)
        {
            // TODO: 'return inCompilerServiceExplorerState with {};' is hacky way of forcing re-render (2023-09-07)
            return inCompilerServiceExplorerState with {};
        }
    }
}