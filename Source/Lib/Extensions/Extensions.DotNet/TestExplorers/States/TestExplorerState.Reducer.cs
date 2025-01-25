using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.States;

public partial record TestExplorerState
{
    public class Reducer
    {
        [ReducerMethod]
        public static TestExplorerState ReduceWithAction(
            TestExplorerState inState,
            WithAction withAction)
        {
            return withAction.WithFunc.Invoke(inState);
        }
        
        [ReducerMethod]
        public static TestExplorerState ReduceInitializeResizeHandleDimensionUnitAction(
            TestExplorerState inState,
            InitializeResizeHandleDimensionUnitAction initializeResizeHandleDimensionUnitAction)
        {
            if (initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose != DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN)
            	return inState;
            
            // TreeViewElementDimensions
            {
            	if (inState.TreeViewElementDimensions.WidthDimensionAttribute.DimensionUnitList is null)
            		return inState;
            		
            	var existingDimensionUnit = inState.TreeViewElementDimensions.WidthDimensionAttribute.DimensionUnitList
            		.FirstOrDefault(x => x.Purpose == initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose);
            		
	            if (existingDimensionUnit.Purpose is not null)
	            	return inState;
            		
            	inState.TreeViewElementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(initializeResizeHandleDimensionUnitAction.DimensionUnit);
            }
            
            // DetailsElementDimensions
            {
            	if (inState.DetailsElementDimensions.WidthDimensionAttribute.DimensionUnitList is null)
            		return inState;
            		
            	var existingDimensionUnit = inState.DetailsElementDimensions.WidthDimensionAttribute.DimensionUnitList
            		.FirstOrDefault(x => x.Purpose == initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose);
            		
	            if (existingDimensionUnit.Purpose is not null)
	            	return inState;
            		
            	inState.DetailsElementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(initializeResizeHandleDimensionUnitAction.DimensionUnit);
            }
            
            return inState;
        }
    }
}