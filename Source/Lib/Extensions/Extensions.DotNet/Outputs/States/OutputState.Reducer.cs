using Fluxor;

namespace Luthetus.Extensions.DotNet.Outputs.States;

public partial record OutputState
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
