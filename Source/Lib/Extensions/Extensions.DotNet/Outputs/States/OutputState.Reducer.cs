using Fluxor;

namespace Luthetus.Extensions.DotNet.Outputs.States;

public partial record OutputState
{
	public class Reducer
    {
        [ReducerMethod]
        public static OutputState ReduceStateHasChangedAction(
            OutputState inState,
            StateHasChangedAction stateHasChangedAction)
        {
            return inState with
            {
            	DotNetRunParseResultId = stateHasChangedAction.DotNetRunParseResultId
            };
        }
    }
}
