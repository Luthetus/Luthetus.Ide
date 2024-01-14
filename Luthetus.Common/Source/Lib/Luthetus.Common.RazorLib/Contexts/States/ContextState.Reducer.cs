using Fluxor;
using Luthetus.Common.RazorLib.Contexts.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Contexts.States;

public partial record ContextState
{
    public class Reducer
    {
        [ReducerMethod]
        public static ContextState ReduceSetFocusedContextHeirarchyAction(
            ContextState inContextStates,
            SetFocusedContextHeirarchyAction setFocusedContextHeirarchyAction)
        {
            return inContextStates with
            {
                FocusedContextHeirarchy = setFocusedContextHeirarchyAction.FocusedContextHeirarchy
            };
        }
        
        [ReducerMethod(typeof(ToggleSelectInspectedContextHeirarchyAction))]
        public static ContextState ReduceToggleSelectInspectedContextHeirarchyAction(
            ContextState inContextStates)
        {
            var outIsSelectingInspectionTarget = !inContextStates.IsSelectingInspectionTarget;

            var outInspectableContextList = inContextStates.InspectableContextList;
            var outInspectedContextHeirarchy = inContextStates.InspectedContextHeirarchy;

            if (!outIsSelectingInspectionTarget)
            {
                outInspectableContextList = ImmutableArray<InspectableContext>.Empty;
                outInspectedContextHeirarchy = null;
            }

            return inContextStates with
            {
                IsSelectingInspectionTarget = !inContextStates.IsSelectingInspectionTarget,
                InspectableContextList = outInspectableContextList,
                InspectedContextHeirarchy = outInspectedContextHeirarchy,
            };
        }
        
        [ReducerMethod]
        public static ContextState ReduceIsSelectingInspectableContextHeirarchyAction(
            ContextState inContextStates,
            IsSelectingInspectableContextHeirarchyAction isSelectingInspectableContextHeirarchyAction)
        {
            if (isSelectingInspectableContextHeirarchyAction.Value)
            {
                return inContextStates with
                {
                    IsSelectingInspectionTarget = true
                };
            }
            else
            {
                return inContextStates with
                {
                    IsSelectingInspectionTarget = false,
                    InspectableContextList = ImmutableArray<InspectableContext>.Empty,
                    InspectedContextHeirarchy = null,
                };
            }
        }
        
        [ReducerMethod]
        public static ContextState ReduceSetInspectedContextHeirarchyAction(
            ContextState inContextStates,
            SetInspectedContextHeirarchyAction setInspectedContextHeirarchyAction)
        {
            return inContextStates with
            {
                IsSelectingInspectionTarget = false,
                InspectableContextList = ImmutableArray<InspectableContext>.Empty,
                InspectedContextHeirarchy = setInspectedContextHeirarchyAction.InspectedContextHeirarchy,
            };
        }
        
        [ReducerMethod]
        public static ContextState ReduceAddInspectableContextAction(
            ContextState inContextStates,
            AddInspectableContextAction addInspectableContextAction)
        {
            var outList = inContextStates.InspectableContextList.Add(
                addInspectableContextAction.InspectableContext);

            return inContextStates with
            {
                InspectableContextList = outList,
            };
        }
        
        [ReducerMethod]
        public static ContextState ReduceSetContextKeymapAction(
            ContextState inContextStates,
            SetContextKeymapAction setContextKeymapAction)
        {
            var inContextRecord = inContextStates.AllContextsList.FirstOrDefault(
                x => x.ContextKey == setContextKeymapAction.ContextKey);

            if (inContextRecord is null)
                return inContextStates;

            var outList = inContextStates.AllContextsList.Replace(inContextRecord, inContextRecord with
            {
                Keymap = setContextKeymapAction.Keymap
            });

            return inContextStates with
            {
                AllContextsList = outList,
            };
        }
    }
}