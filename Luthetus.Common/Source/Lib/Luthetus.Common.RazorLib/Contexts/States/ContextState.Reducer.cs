using Fluxor;
using Luthetus.Common.RazorLib.Contexts.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Contexts.States;

public partial record ContextState
{
    private class Reducer
    {
        [ReducerMethod]
        public static ContextState ReduceSetActiveContextRecordsAction(
            ContextState inContextStates,
            SetActiveContextRecordsAction setActiveContextRecordsAction)
        {
            return inContextStates with
            {
                FocusedContextRecordKeyHeirarchy = setActiveContextRecordsAction.ContextRecordKeyHeirarchy
            };
        }
        
        [ReducerMethod(typeof(ToggleSelectInspectionTargetAction))]
        public static ContextState ReduceToggleInspectAction(
            ContextState inContextStates)
        {
            var outIsSelectingInspectionTarget = !inContextStates.IsSelectingInspectionTarget;

            var outInspectContextRecordEntryBag = inContextStates.InspectContextRecordEntryBag;
            var outInspectedTargetContextRecords = inContextStates.InspectedContextRecordKeyHeirarchy;

            if (!outIsSelectingInspectionTarget)
            {
                outInspectContextRecordEntryBag = ImmutableArray<InspectContextRecordEntry>.Empty;
                outInspectedTargetContextRecords = null;
            }

            return inContextStates with
            {
                IsSelectingInspectionTarget = !inContextStates.IsSelectingInspectionTarget,
                InspectedContextRecordKeyHeirarchy = outInspectedTargetContextRecords,
                InspectContextRecordEntryBag = outInspectContextRecordEntryBag,
            };
        }
        
        [ReducerMethod(typeof(SetSelectInspectionTargetTrueAction))]
        public static ContextState ReduceSetSelectInspectionTargetTrueAction(
            ContextState inContextStates)
        {
            return inContextStates with
            {
                IsSelectingInspectionTarget = true
            };
        }
        
        [ReducerMethod(typeof(SetSelectInspectionTargetFalseAction))]
        public static ContextState ReduceSetSelectInspectionTargetFalseAction(
            ContextState inContextStates)
        {
            return inContextStates with
            {
                IsSelectingInspectionTarget = false,
                InspectedContextRecordKeyHeirarchy = null,
                InspectContextRecordEntryBag = ImmutableArray<InspectContextRecordEntry>.Empty,
            };
        }
        
        [ReducerMethod]
        public static ContextState ReduceSetInspectionTargetAction(
            ContextState inContextStates,
            SetInspectionTargetAction setInspectionTargetAction)
        {
            return inContextStates with
            {
                IsSelectingInspectionTarget = false,
                InspectedContextRecordKeyHeirarchy = setInspectionTargetAction.ContextRecordKeyHeirarchy,
                InspectContextRecordEntryBag = ImmutableArray<InspectContextRecordEntry>.Empty
            };
        }
        
        [ReducerMethod]
        public static ContextState ReduceAddInspectContextRecordEntryAction(
            ContextState inContextStates,
            AddInspectContextRecordEntryAction addInspectContextRecordEntryAction)
        {
            var outList = inContextStates.InspectContextRecordEntryBag.Add(
                addInspectContextRecordEntryAction.InspectContextRecordEntry);

            return inContextStates with
            {
                InspectContextRecordEntryBag = outList,
            };
        }
        
        [ReducerMethod]
        public static ContextState ReduceSetContextKeymapAction(
            ContextState inContextStates,
            SetContextKeymapAction setContextKeymapAction)
        {
            var inContextRecord = inContextStates.AllContextRecordsBag.FirstOrDefault(
                x => x.ContextKey == setContextKeymapAction.ContextRecordKey);

            if (inContextRecord is null)
                return inContextStates;

            var outList = inContextStates.AllContextRecordsBag.Replace(inContextRecord, inContextRecord with
            {
                Keymap = setContextKeymapAction.Keymap
            });

            return inContextStates with
            {
                AllContextRecordsBag = outList,
            };
        }
    }
}