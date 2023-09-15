using Fluxor;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States;

public partial class CompilerServiceExplorerRegistry
{
    private class Reducer
    {
        [ReducerMethod]
        public CompilerServiceExplorerRegistry ReduceSetCompilerServiceExplorerStateAction(
            CompilerServiceExplorerRegistry inCompilerServiceExplorerState,
            SetCompilerServiceExplorerAction setCompilerServiceExplorerStateAction)
        {
            // TODO: 'return new CompilerServiceExplorerState(...);' is hacky way of forcing re-render (2023-09-07)
            return new CompilerServiceExplorerRegistry(
                inCompilerServiceExplorerState.Model,
                inCompilerServiceExplorerState.GraphicalView,
                inCompilerServiceExplorerState.ReflectionView);
        }
    }
}