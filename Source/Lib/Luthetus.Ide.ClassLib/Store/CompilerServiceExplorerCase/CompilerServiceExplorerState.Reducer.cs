using Fluxor;

namespace Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase;

public partial class CompilerServiceExplorerState
{
    private class Reducer
    {
        [ReducerMethod]
        public CompilerServiceExplorerState ReduceSetCompilerServiceExplorerStateAction(
            CompilerServiceExplorerState inCompilerServiceExplorerState,
            SetCompilerServiceExplorerAction setCompilerServiceExplorerStateAction)
        {
            // TODO: 'return new CompilerServiceExplorerState(...);' is hacky way of forcing re-render (2023-09-07)
            return new CompilerServiceExplorerState(
                inCompilerServiceExplorerState.Model,
                inCompilerServiceExplorerState.GraphicalView,
                inCompilerServiceExplorerState.ReflectionView);
        }
    }
}