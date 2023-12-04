using Fluxor;

namespace Luthetus.Ide.RazorLib.TestExplorers.States;

public partial record TestExplorerState
{
    public class Reducer
    {
        [ReducerMethod]
        public static TestExplorerState ReduceWithAction(
            TestExplorerState inState,
            WithAction withAction)
        {
            return withAction.WithFunc.Invoke(inState);
        }
    }
}