using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;

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

            var outPanelList = inState.PanelList.Add(registerPanelAction.Panel);

            return inState with { PanelList = outPanelList };
        }

        [ReducerMethod]
        public static PanelState ReduceDisposePanelAction(
            PanelState inState,
            DisposePanelAction disposePanelAction)
        {
            var inPanel = inState.PanelList.FirstOrDefault(
                x => x.Key == disposePanelAction.PanelKey);

            if (inPanel is null)
                return inState;

            var outPanelList = inState.PanelList.Remove(inPanel);

            return inState with { PanelList = outPanelList };
        }
		
		[ReducerMethod]
        public static PanelState ReduceRegisterPanelGroupAction(
            PanelState inState,
            RegisterPanelGroupAction registerPanelGroupAction)
        {
            if (inState.PanelGroupList.Any(x => x.Key == registerPanelGroupAction.PanelGroup.Key))
                return inState;

            var outPanelGroupList = inState.PanelGroupList.Add(registerPanelGroupAction.PanelGroup);

            return inState with { PanelGroupList = outPanelGroupList };
        }

        [ReducerMethod]
        public static PanelState ReduceDisposePanelGroupAction(
            PanelState inState,
            DisposePanelGroupAction disposePanelGroupAction)
        {
            var inPanelGroup = inState.PanelGroupList.FirstOrDefault(
                x => x.Key == disposePanelGroupAction.PanelGroupKey);

            if (inPanelGroup is null)
                return inState;

            var outPanelGroupList = inState.PanelGroupList.Remove(inPanelGroup);

            return inState with { PanelGroupList = outPanelGroupList };
        }

        [ReducerMethod]
        public static PanelState ReduceRegisterPanelTabAction(
            PanelState inState,
            RegisterPanelTabAction registerPanelTabAction)
        {
            var inPanelGroup = inState.PanelGroupList.FirstOrDefault(
                x => x.Key == registerPanelTabAction.PanelGroupKey);

            if (inPanelGroup is null)
                return inState;

            var outTabList = inPanelGroup.TabList;

            var insertionPoint = registerPanelTabAction.InsertAtIndexZero
                ? 0
                : inPanelGroup.TabList.Length;

            outTabList = inPanelGroup.TabList.Insert(insertionPoint, registerPanelTabAction.PanelTab);

            var outPanelGroupList = inState.PanelGroupList.Replace(inPanelGroup, inPanelGroup with
            {
                TabList = outTabList
            });

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
            var inPanelGroup = inState.PanelGroupList.FirstOrDefault(
                x => x.Key == disposePanelTabAction.PanelGroupKey);

            if (inPanelGroup is null)
                return inState;

            var panelTab = inPanelGroup.TabList.FirstOrDefault(
                x => x.Key == disposePanelTabAction.PanelTabKey);

            if (panelTab is null)
                return inState;

            var outTabList = inPanelGroup.TabList.Remove(panelTab);

            var outPanelGroupList = inState.PanelGroupList.Replace(inPanelGroup, inPanelGroup with
            {
                TabList = outTabList
            });

            return inState with { PanelGroupList = outPanelGroupList };
        }

        [ReducerMethod]
        public static PanelState ReduceSetActivePanelTabAction(
            PanelState inState,
            SetActivePanelTabAction setActivePanelTabAction)
        {
            var inPanelGroup = inState.PanelGroupList.FirstOrDefault(
                x => x.Key == setActivePanelTabAction.PanelGroupKey);

            if (inPanelGroup is null)
                return inState;

            var outPanelGroupList = inState.PanelGroupList.Replace(inPanelGroup, inPanelGroup with
            {
                ActiveTabKey = setActivePanelTabAction.PanelTabKey
            });

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