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
            	var widthDimensionAttribute = inState.TreeViewElementDimensions.DimensionAttributeList.FirstOrDefault(
            		x => x.DimensionAttributeKind == DimensionAttributeKind.Width);
            		
            	if (widthDimensionAttribute.DimensionUnitList is null)
            		return inState;
            		
            	var existingDimensionUnit = widthDimensionAttribute.DimensionUnitList.FirstOrDefault(x => x.Purpose == initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose);
	            if (existingDimensionUnit.Purpose is not null)
	            	return inState;
            		
            	widthDimensionAttribute.DimensionUnitList.Add(initializeResizeHandleDimensionUnitAction.DimensionUnit);
            }
            
            // DetailsElementDimensions
            {
            	var widthDimensionAttribute = inState.DetailsElementDimensions.DimensionAttributeList.FirstOrDefault(
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