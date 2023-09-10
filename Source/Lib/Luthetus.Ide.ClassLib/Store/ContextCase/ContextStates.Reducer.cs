using Fluxor;
using System.Collections.Immutable;

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
            var outIsSelectingInspectionTarget = !inContextStates.IsSelectingInspectionTarget;

            var outMeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples = inContextStates.MeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples;
            var outInspectionTargetContextRecords = inContextStates.InspectionTargetContextRecords;

            if (!outIsSelectingInspectionTarget)
            {
                outMeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples = ImmutableArray<(Context.ContextRecord contextRecord, System.Collections.Immutable.ImmutableArray<Context.ContextRecord> contextBoundaryHeirarchy, JavaScriptObjects.MeasuredHtmlElementDimensions measuredHtmlElementDimensions)>.Empty;
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
        public static ContextStates ReduceSetSelectInspectionTargetTrueAction(
            ContextStates inContextStates)
        {
            return inContextStates with
            {
                IsSelectingInspectionTarget = true
            };
        }
        
        [ReducerMethod(typeof(SetSelectInspectionTargetFalseAction))]
        public static ContextStates ReduceSetSelectInspectionTargetFalseAction(
            ContextStates inContextStates)
        {
            return inContextStates with
            {
                IsSelectingInspectionTarget = false,
                InspectionTargetContextRecords = null,
                MeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples = ImmutableArray<(Context.ContextRecord contextRecord, System.Collections.Immutable.ImmutableArray<Context.ContextRecord> contextBoundaryHeirarchy, JavaScriptObjects.MeasuredHtmlElementDimensions measuredHtmlElementDimensions)>.Empty,
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
                IsSelectingInspectionTarget = false,
                MeasuredHtmlElementDimensionsForSelectingInspectionTargetTuples = System.Collections.Immutable.ImmutableArray<(Context.ContextRecord contextRecord, System.Collections.Immutable.ImmutableArray<Context.ContextRecord> contextBoundaryHeirarchy, JavaScriptObjects.MeasuredHtmlElementDimensions measuredHtmlElementDimensions)>.Empty
            };
        }
        
        [ReducerMethod]
        public static ContextStates ReduceSetMeasuredHtmlElementDimensionsAction(
            ContextStates inContextStates,
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