using Fluxor;

namespace Luthetus.Ide.ClassLib.Store.ContextCase;

public partial record ContextStates
{
    private class Reducer
    {
        [ReducerMethod]
        public static ContextStates ReduceSetActiveContextRecordsAction(
            ContextStates inContextStates,
            SetActiveContextRecordsAction setActiveContextRecordsAction)
        {
            return inContextStates with
            {
                ActiveContextRecords = setActiveContextRecordsAction.ContextRecords
            };
        }
        
        [ReducerMethod(typeof(ToggleSelectInspectionTargetAction))]
        public static ContextStates ReduceToggleInspectAction(
            ContextStates inContextStates)
        {
            return inContextStates with
            {
                IsSelectingInspectionTarget = !inContextStates.IsSelectingInspectionTarget,
            };
        }
        
        [ReducerMethod]
        public static ContextStates ReduceSetInspectionTargetAction(
            ContextStates inContextStates,
            SetInspectionTargetAction setInspectionTargetAction)
        {
            return inContextStates with
            {
                InspectionTargetContextRecords = setInspectionTargetAction.ContextRecords,
            };
        }
        
        [ReducerMethod]
        public static ContextStates ReduceSetMeasuredHtmlElementDimensionsAction(
            ContextStates inContextStates,
            AddMeasuredHtmlElementDimensionsAction addMeasuredHtmlElementDimensionsAction)
        {
            var outList = inContextStates.MeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples
                .Add((addMeasuredHtmlElementDimensionsAction.ContextRecord, addMeasuredHtmlElementDimensionsAction.MeasuredHtmlElementDimensions));

            return inContextStates with
            {
                MeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples = outList,
            };
        }
    }
}