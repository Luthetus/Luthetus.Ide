using Fluxor;

namespace Luthetus.Ide.ClassLib.Store.ContextCase;

public partial record ContextStates
{
    private class Reducer
    {
        [ReducerMethod]
        public static ContextStates ReduceSetActiveContextRecordsAction(
            ContextStates previousContextStates,
            SetActiveContextRecordsAction setActiveContextRecordsAction)
        {
            return previousContextStates with
            {
                ActiveContextRecords = setActiveContextRecordsAction.ContextRecords
            };
        }
    }
}