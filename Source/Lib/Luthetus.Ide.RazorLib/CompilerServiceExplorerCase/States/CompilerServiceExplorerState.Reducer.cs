using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States;

public partial class CompilerServiceExplorerState
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