using Fluxor;

namespace Luthetus.Ide.Tests.Basis.TestExplorers.States;

public partial record TestExplorerStateReducerTests
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