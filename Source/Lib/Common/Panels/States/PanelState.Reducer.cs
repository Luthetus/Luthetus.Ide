using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Panels.Models;

namespace Luthetus.Common.RazorLib.Panels.States;

public partial record PanelState
{
    public class Reducer
    {
        [ReducerMethod]
        public static PanelState ReduceRegisterPanelAction(
            PanelState inState,
            RegisterPanelAction registerPanelAction)
        {
            if (inState.PanelList.Any(x => x.Key == registerPanelAction.Panel.Key))
                return inState;

            var outPanelList = new List<Panel>(inState.PanelList);
            outPanelList.Add(registerPanelAction.Panel);

            return inState with { PanelList = outPanelList };
        }

        [ReducerMethod]
        public static PanelState ReduceDisposePanelAction(
            PanelState inState,
            DisposePanelAction disposePanelAction)
        {
            var indexPanel = inState.PanelList.FindIndex(
                x => x.Key == disposePanelAction.PanelKey);

            if (indexPanel == -1)
                return inState;

            var outPanelList = new List<Panel>(inState.PanelList);
            outPanelList.RemoveAt(indexPanel);

            return inState with { PanelList = outPanelList };
        }
		
		[ReducerMethod]
        public static PanelState ReduceRegisterPanelGroupAction(
            PanelState inState,
            RegisterPanelGroupAction registerPanelGroupAction)
        {
            if (inState.PanelGroupList.Any(x => x.Key == registerPanelGroupAction.PanelGroup.Key))
                return inState;

            var outPanelGroupList = new List<PanelGroup>(inState.PanelGroupList);
            outPanelGroupList.Add(registerPanelGroupAction.PanelGroup);
            
            return inState with { PanelGroupList = outPanelGroupList };
        }

        [ReducerMethod]
        public static PanelState ReduceDisposePanelGroupAction(
            PanelState inState,
            DisposePanelGroupAction disposePanelGroupAction)
        {
            var indexPanelGroup = inState.PanelGroupList.FindIndex(
                x => x.Key == disposePanelGroupAction.PanelGroupKey);

            if (indexPanelGroup == -1)
                return inState;

            var outPanelGroupList = new List<PanelGroup>(inState.PanelGroupList);
            outPanelGroupList.RemoveAt(indexPanelGroup);

            return inState with { PanelGroupList = outPanelGroupList };
        }

        [ReducerMethod]
        public static PanelState ReduceRegisterPanelTabAction(
            PanelState inState,
            RegisterPanelTabAction registerPanelTabAction)
        {
            var indexPanelGroup = inState.PanelGroupList.FindIndex(
                x => x.Key == registerPanelTabAction.PanelGroupKey);

            if (indexPanelGroup == -1)
                return inState;
                
            var inPanelGroup = inState.PanelGroupList[indexPanelGroup];

            var outTabList = new List<IPanelTab>(inPanelGroup.TabList);

            var insertionPoint = registerPanelTabAction.InsertAtIndexZero
                ? 0
                : outTabList.Count;

            outTabList.Insert(insertionPoint, registerPanelTabAction.PanelTab);

            var outPanelGroupList = new List<PanelGroup>(inState.PanelGroupList);
            
            outPanelGroupList[indexPanelGroup] = inPanelGroup with
            {
                TabList = outTabList
            };

            return inState with
            {
                PanelGroupList = outPanelGroupList
            };
        }

        [ReducerMethod]
        public static PanelState ReduceDisposePanelTabAction(
            PanelState inState,
            DisposePanelTabAction disposePanelTabAction)
        {
            var indexPanelGroup = inState.PanelGroupList.FindIndex(
                x => x.Key == disposePanelTabAction.PanelGroupKey);

            if (indexPanelGroup == -1)
                return inState;
                
            var inPanelGroup = inState.PanelGroupList[indexPanelGroup];

            var indexPanelTab = inPanelGroup.TabList.FindIndex(
                x => x.Key == disposePanelTabAction.PanelTabKey);

            if (indexPanelTab == -1)
                return inState;

            var outTabList = new List<IPanelTab>(inPanelGroup.TabList);
            outTabList.RemoveAt(indexPanelTab);
            
            var outPanelGroupList = new List<PanelGroup>(inState.PanelGroupList);
            outPanelGroupList[indexPanelGroup] = inPanelGroup with
            {
                TabList = outTabList
            };

            return inState with { PanelGroupList = outPanelGroupList };
        }

        [ReducerMethod]
        public static PanelState ReduceSetActivePanelTabAction(
            PanelState inState,
            SetActivePanelTabAction setActivePanelTabAction)
        {
            var indexPanelGroup = inState.PanelGroupList.FindIndex(
                x => x.Key == setActivePanelTabAction.PanelGroupKey);

            if (indexPanelGroup == -1)
                return inState;
                
            var inPanelGroup = inState.PanelGroupList[indexPanelGroup];

			var outPanelGroupList = new List<PanelGroup>(inState.PanelGroupList);
			
            outPanelGroupList[indexPanelGroup] = inPanelGroup with
            {
                ActiveTabKey = setActivePanelTabAction.PanelTabKey
            };

            return inState with { PanelGroupList = outPanelGroupList };
        }

        [ReducerMethod]
        public static PanelState ReduceSetPanelTabAsActiveByContextRecordKeyAction(
            PanelState inState,
            SetPanelTabAsActiveByContextRecordKeyAction setPanelTabAsActiveByContextRecordKeyAction)
        {
            var inPanelGroup = inState.PanelGroupList.FirstOrDefault(x => x.TabList
                .Any(y => y.ContextRecordKey == setPanelTabAsActiveByContextRecordKeyAction.ContextRecordKey));

            if (inPanelGroup is null)
                return inState;

            var inPanelTab = inPanelGroup.TabList.FirstOrDefault(
                x => x.ContextRecordKey == setPanelTabAsActiveByContextRecordKeyAction.ContextRecordKey);

            if (inPanelTab is null)
                return inState;

            return ReduceSetActivePanelTabAction(inState, new SetActivePanelTabAction(
                inPanelGroup.Key,
                inPanelTab.Key));
        }

        [ReducerMethod]
        public static PanelState ReduceSetDragEventArgsAction(
            PanelState inState,
            SetDragEventArgsAction setDragEventArgsAction)
        {
            return inState with
            {
                DragEventArgs = setDragEventArgsAction.DragEventArgs
            };
        }
        
        [ReducerMethod]
        public static PanelState ReduceInitializeResizeHandleDimensionUnitAction(
            PanelState inState,
            InitializeResizeHandleDimensionUnitAction initializeResizeHandleDimensionUnitAction)
        {
            var inPanelGroup = inState.PanelGroupList.FirstOrDefault(
                x => x.Key == initializeResizeHandleDimensionUnitAction.PanelGroupKey);

            if (inPanelGroup is null)
                return inState;
                
            if (initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose != DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_ROW ||
            	initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose != DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN)
            {
            	return inState;
            }
                
			if (initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose == DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_ROW)
            {
            	if (inPanelGroup.ElementDimensions.HeightDimensionAttribute.DimensionUnitList is null)
            		return inState;
            		
				var existingDimensionUnit = inPanelGroup.ElementDimensions.HeightDimensionAttribute.DimensionUnitList
					.FirstOrDefault(x => x.Purpose == initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose);
					
	            if (existingDimensionUnit.Purpose is not null)
	            	return inState;
            		
            	inPanelGroup.ElementDimensions.HeightDimensionAttribute.DimensionUnitList.Add(initializeResizeHandleDimensionUnitAction.DimensionUnit);
            }
            else if (initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose != DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN)
            {
            	if (inPanelGroup.ElementDimensions.WidthDimensionAttribute.DimensionUnitList is null)
            		return inState;
            		
            	var existingDimensionUnit = inPanelGroup.ElementDimensions.WidthDimensionAttribute.DimensionUnitList
            		.FirstOrDefault(x => x.Purpose == initializeResizeHandleDimensionUnitAction.DimensionUnit.Purpose);
	            
	            if (existingDimensionUnit.Purpose is not null)
	            	return inState;
            		
            	inPanelGroup.ElementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(initializeResizeHandleDimensionUnitAction.DimensionUnit);
            }
            
            return inState;
        }
    }
}