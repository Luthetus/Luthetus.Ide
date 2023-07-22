using Fluxor;

namespace Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;

public partial record DotNetSolutionState
{
    private class Reducer
    {
        [ReducerMethod]
        public DotNetSolutionState ReduceWithAction(
            DotNetSolutionState inDotNetSolutionState,
            WithAction withAction)
        {
            return withAction.WithFunc
                .Invoke(inDotNetSolutionState);
        }
    }
}