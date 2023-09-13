using Fluxor;
using Luthetus.Ide.RazorLib.ContextCase;
using Luthetus.Ide.RazorLib.JavaScriptObjectsCase;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.Store.ContextCase;

public partial record ContextRegistry
{
    private class Reducer
    {
        [ReducerMethod]
        public static ContextRegistry ReduceSetActiveContextRecordsAction(
            ContextRegistry inContextStates,
            SetActiveContextRecordsAction setActiveContextRecordsAction)
        {
            return inContextStates with
            {
                ActiveContextRecords = setActiveContextRecordsAction.ContextRecords
            };
        }
        
        [ReducerMethod(typeof(ToggleSelectInspectionTargetAction))]
        public static ContextRegistry ReduceToggleInspectAction(
            ContextRegistry inContextStates)
        {
            var outIsSelectingInspectionTarget = !inContextStates.IsSelectingInspectionTarget;

            var outMeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples = inContextStates.MeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples;
            var outInspectionTargetContextRecords = inContextStates.InspectionTargetContextRecords;

            if (!outIsSelectingInspectionTarget)
            {
                outMeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples = ImmutableArray<(ContextRecord contextRecord, ImmutableArray<ContextRecord> contextBoundaryHeirarchy, MeasuredHtmlElementDimensions measuredHtmlElementDimensions)>.Empty;
                outInspectionTargetContextRecords = null;
            }

            return inContextStates with
            {
                IsSelectingInspectionTarget = !inContextStates.IsSelectingInspectionTarget,
                InspectionTargetContextRecords = outInspectionTargetContextRecords,
                MeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples = outMeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples,
            };
        }
        
        [ReducerMethod(typeof(SetSelectInspectionTargetTrueAction))]
        public static ContextRegistry ReduceSetSelectInspectionTargetTrueAction(
            ContextRegistry inContextStates)
        {
            return inContextStates with
            {
                IsSelectingInspectionTarget = true
            };
        }
        
        [ReducerMethod(typeof(SetSelectInspectionTargetFalseAction))]
        public static ContextRegistry ReduceSetSelectInspectionTargetFalseAction(
            ContextRegistry inContextStates)
        {
            return inContextStates with
            {
                IsSelectingInspectionTarget = false,
                InspectionTargetContextRecords = null,
                MeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples = ImmutableArray<(ContextRecord contextRecord, ImmutableArray<ContextRecord> contextBoundaryHeirarchy, MeasuredHtmlElementDimensions measuredHtmlElementDimensions)>.Empty,
            };
        }
        
        [ReducerMethod]
        public static ContextRegistry ReduceSetInspectionTargetAction(
            ContextRegistry inContextStates,
            SetInspectionTargetAction setInspectionTargetAction)
        {
            return inContextStates with
            {
                InspectionTargetContextRecords = setInspectionTargetAction.ContextRecords,
                IsSelectingInspectionTarget = false,
                MeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples = ImmutableArray<(ContextRecord contextRecord, ImmutableArray<ContextRecord> contextBoundaryHeirarchy, MeasuredHtmlElementDimensions measuredHtmlElementDimensions)>.Empty
            };
        }
        
        [ReducerMethod]
        public static ContextRegistry ReduceSetMeasuredHtmlElementDimensionsAction(
            ContextRegistry inContextStates,
            AddMeasuredHtmlElementDimensionsAction addMeasuredHtmlElementDimensionsAction)
        {
            var outList = inContextStates.MeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples
                .Add((addMeasuredHtmlElementDimensionsAction.ContextRecord, addMeasuredHtmlElementDimensionsAction.ContextBoundaryHeirarchy, addMeasuredHtmlElementDimensionsAction.MeasuredHtmlElementDimensions));

            return inContextStates with
            {
                MeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples = outList,
            };
        }
    }
}