using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Contexts.Models;

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
                inContextStates.InspectableContextList.Clear();
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
            	inContextStates.InspectableContextList.Clear();
            	
                return inContextStates with
                {
                    IsSelectingInspectionTarget = false,
                    InspectedContextHeirarchy = null,
                };
            }
        }
        
        [ReducerMethod]
        public static ContextState ReduceSetInspectedContextHeirarchyAction(
            ContextState inContextStates,
            SetInspectedContextHeirarchyAction setInspectedContextHeirarchyAction)
        {
        	inContextStates.InspectableContextList.Clear();
        
            return inContextStates with
            {
                IsSelectingInspectionTarget = false,
                InspectedContextHeirarchy = setInspectedContextHeirarchyAction.InspectedContextHeirarchy,
            };
        }
        
        [ReducerMethod]
        public static ContextState ReduceAddInspectableContextAction(
            ContextState inContextStates,
            AddInspectableContextAction addInspectableContextAction)
        {
            inContextStates.InspectableContextList.Add(
                addInspectableContextAction.InspectableContext);

            return inContextStates with { };
        }
        
        [ReducerMethod]
        public static ContextState ReduceSetContextKeymapAction(
            ContextState inContextStates,
            SetContextKeymapAction setContextKeymapAction)
        {
            var inContextRecord = inContextStates.AllContextsList.FirstOrDefault(
                x => x.ContextKey == setContextKeymapAction.ContextKey);

            if (inContextRecord != default)
                return inContextStates;
                
            var index = inContextStates.AllContextsList.FindIndex(x => x.ContextKey == inContextRecord.ContextKey);
            if (index == -1)
            	return inContextStates;

            inContextStates.AllContextsList[index] = inContextRecord with
            {
                Keymap = setContextKeymapAction.Keymap
            };

            return inContextStates with { };
        }
    }
}