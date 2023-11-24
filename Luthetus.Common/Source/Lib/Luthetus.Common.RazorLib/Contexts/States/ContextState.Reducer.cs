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

            var outInspectableContextBag = inContextStates.InspectableContextBag;
            var outInspectedContextHeirarchy = inContextStates.InspectedContextHeirarchy;

            if (!outIsSelectingInspectionTarget)
            {
                outInspectableContextBag = ImmutableArray<InspectableContext>.Empty;
                outInspectedContextHeirarchy = null;
            }

            return inContextStates with
            {
                IsSelectingInspectionTarget = !inContextStates.IsSelectingInspectionTarget,
                InspectableContextBag = outInspectableContextBag,
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
                    InspectableContextBag = ImmutableArray<InspectableContext>.Empty,
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
                InspectableContextBag = ImmutableArray<InspectableContext>.Empty,
                InspectedContextHeirarchy = setInspectedContextHeirarchyAction.InspectedContextHeirarchy,
            };
        }
        
        [ReducerMethod]
        public static ContextState ReduceAddInspectableContextAction(
            ContextState inContextStates,
            AddInspectableContextAction addInspectableContextAction)
        {
            var outList = inContextStates.InspectableContextBag.Add(
                addInspectableContextAction.InspectableContext);

            return inContextStates with
            {
                InspectableContextBag = outList,
            };
        }
        
        [ReducerMethod]
        public static ContextState ReduceSetContextKeymapAction(
            ContextState inContextStates,
            SetContextKeymapAction setContextKeymapAction)
        {
            var inContextRecord = inContextStates.AllContextsBag.FirstOrDefault(
                x => x.ContextKey == setContextKeymapAction.ContextKey);

            if (inContextRecord is null)
                return inContextStates;

            var outList = inContextStates.AllContextsBag.Replace(inContextRecord, inContextRecord with
            {
                Keymap = setContextKeymapAction.Keymap
            });

            return inContextStates with
            {
                AllContextsBag = outList,
            };
        }
    }
}