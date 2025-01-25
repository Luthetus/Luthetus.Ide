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
            	var widthDimensionAttribute = inState.BodyElementDimensions.DimensionAttributeList.FirstOrDefault(
            		x => x.DimensionAttributeKind == DimensionAttributeKind.Width);
            		
            	if (widthDimensionAttribute.DimensionUnitList is null)
            		return inState;
            		
            	var existingDimensionUnit = widthDimensionAttribute.DimensionUnitList.FirstOrDefault(x => x.Purpose == initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose);
	            if (existingDimensionUnit.Purpose is not null)
	            	return inState;
            		
            	widthDimensionAttribute.DimensionUnitList.Add(initializeResizeHandleDimensionUnitAction.DimensionUnit);
            }
            
            // TabsElementDimensions
            {
            	var widthDimensionAttribute = inState.TabsElementDimensions.DimensionAttributeList.FirstOrDefault(
            		x => x.DimensionAttributeKind == DimensionAttributeKind.Width);
            		
            	if (widthDimensionAttribute.DimensionUnitList is null)
            		return inState;
            		
            	var existingDimensionUnit = widthDimensionAttribute.DimensionUnitList.FirstOrDefault(x => x.Purpose == initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose);
	            if (existingDimensionUnit.Purpose is not null)
	            	return inState;
            		
            	widthDimensionAttribute.DimensionUnitList.Add(initializeResizeHandleDimensionUnitAction.DimensionUnit);
            }
            
            return inState;
        }
    }
}
