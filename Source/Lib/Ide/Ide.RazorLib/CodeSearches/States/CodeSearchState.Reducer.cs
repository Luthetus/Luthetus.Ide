using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Ide.RazorLib.CodeSearches.Models;

namespace Luthetus.Ide.RazorLib.CodeSearches.States;

public partial record CodeSearchState
{
    public class Reducer
    {
        [ReducerMethod]
        public static CodeSearchState ReduceWithAction(
            CodeSearchState inState,
            WithAction withAction)
        {
            var outState = withAction.WithFunc.Invoke(inState);

            if (outState.Query.StartsWith("f:"))
            {
                outState = outState with
                {
                    CodeSearchFilterKind = CodeSearchFilterKind.Files
                };
            }
            else if (outState.Query.StartsWith("t:"))
            {
                outState = outState with
                {
                    CodeSearchFilterKind = CodeSearchFilterKind.Types
                };
            }
            else if (outState.Query.StartsWith("m:"))
            {
                outState = outState with
                {
                    CodeSearchFilterKind = CodeSearchFilterKind.Members
                };
            }
            else
            {
                outState = outState with
                {
                    CodeSearchFilterKind = CodeSearchFilterKind.None
                };
            }

            return outState;
        }

        [ReducerMethod]
        public static CodeSearchState ReduceAddResultAction(
            CodeSearchState inState,
            AddResultAction addResultAction)
        {
            return inState with
            {
                ResultList = inState.ResultList.Add(addResultAction.Result)
            };
        }

        [ReducerMethod(typeof(ClearResultListAction))]
        public static CodeSearchState ReduceClearResultListAction(
            CodeSearchState inState)
        {
            return inState with
            {
                ResultList = ImmutableList<string>.Empty
            };
        }
        
        [ReducerMethod]
        public static CodeSearchState ReduceInitializeResizeHandleDimensionUnitAction(
            CodeSearchState inState,
            InitializeResizeHandleDimensionUnitAction initializeResizeHandleDimensionUnitAction)
        {
            if (initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose != DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_ROW)
            	return inState;
            
            // TopContentElementDimensions
            {
            	var heightDimensionAttribute = inState.TopContentElementDimensions.DimensionAttributeList.FirstOrDefault(
            		x => x.DimensionAttributeKind == DimensionAttributeKind.Height);
            		
            	if (heightDimensionAttribute.DimensionUnitList is null)
            		return inState;
            		
            	var existingDimensionUnit = heightDimensionAttribute.DimensionUnitList.FirstOrDefault(x => x.Purpose == initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose);
	            if (existingDimensionUnit.Purpose is not null)
	            	return inState;
            		
            	heightDimensionAttribute.DimensionUnitList.Add(initializeResizeHandleDimensionUnitAction.DimensionUnit);
            }
            
            // BottomContentElementDimensions
            {
            	var heightDimensionAttribute = inState.BottomContentElementDimensions.DimensionAttributeList.FirstOrDefault(
            		x => x.DimensionAttributeKind == DimensionAttributeKind.Height);
            		
            	if (heightDimensionAttribute.DimensionUnitList is null)
            		return inState;
            		
            	var existingDimensionUnit = heightDimensionAttribute.DimensionUnitList.FirstOrDefault(x => x.Purpose == initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose);
	            if (existingDimensionUnit.Purpose is not null)
	            	return inState;
            		
            	heightDimensionAttribute.DimensionUnitList.Add(initializeResizeHandleDimensionUnitAction.DimensionUnit);
            }
            
            return inState;
        }
    }
}
