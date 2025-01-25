using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Ide.RazorLib.Terminals.States;

public partial record class TerminalGroupState
{
    public static class Reducer
    {
        [ReducerMethod]
        public static TerminalGroupState ReduceSetActiveTerminalAction(
            TerminalGroupState inState,
            SetActiveTerminalAction setActiveTerminalAction)
        {
            return inState with
            {
                ActiveTerminalKey = setActiveTerminalAction.TerminalKey
            };
        }
        
        [ReducerMethod]
        public static TerminalGroupState ReduceInitializeResizeHandleDimensionUnitAction(
            TerminalGroupState inState,
            InitializeResizeHandleDimensionUnitAction initializeResizeHandleDimensionUnitAction)
        {
            if (initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose != DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN)
            	return inState;
            
            // BodyElementDimensions
            {
            	if (inState.BodyElementDimensions.WidthDimensionAttribute.DimensionUnitList is null)
            		return inState;
            		
            	var existingDimensionUnit = inState.BodyElementDimensions.WidthDimensionAttribute.DimensionUnitList
            		.FirstOrDefault(x => x.Purpose == initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose);
            		
	            if (existingDimensionUnit.Purpose is not null)
	            	return inState;
            		
            	inState.BodyElementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(initializeResizeHandleDimensionUnitAction.DimensionUnit);
            }
            
            // TabsElementDimensions
            {
            	if (inState.TabsElementDimensions.WidthDimensionAttribute.DimensionUnitList is null)
            		return inState;
            		
            	var existingDimensionUnit = inState.TabsElementDimensions.WidthDimensionAttribute.DimensionUnitList
            		.FirstOrDefault(x => x.Purpose == initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose);
            		
	            if (existingDimensionUnit.Purpose is not null)
	            	return inState;
            		
            	inState.TabsElementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(initializeResizeHandleDimensionUnitAction.DimensionUnit);
            }
            
            return inState;
        }
    }
}
