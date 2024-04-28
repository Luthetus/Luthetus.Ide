using Fluxor;

namespace Luthetus.Common.RazorLib.Panels.States;

public partial record PanelsState
{
    public class Reducer
    {
        [ReducerMethod]
        public static PanelsState ReduceRegisterPanelAction(
            PanelsState inState,
            RegisterPanelAction registerPanelAction)
        {
            if (inState.PanelList.Any(x => x.Key == registerPanelAction.Panel.Key))
                return inState;

            var outPanelList = inState.PanelList.Add(registerPanelAction.Panel);

            return inState with { PanelList = outPanelList };
        }

        [ReducerMethod]
        public static PanelsState ReduceDisposePanelAction(
            PanelsState inState,
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
        public static PanelsState ReduceRegisterPanelGroupAction(
            PanelsState inState,
            RegisterPanelGroupAction registerPanelGroupAction)
        {
            if (inState.PanelGroupList.Any(x => x.Key == registerPanelGroupAction.PanelGroup.Key))
                return inState;

            var outPanelGroupList = inState.PanelGroupList.Add(registerPanelGroupAction.PanelGroup);

            return inState with { PanelGroupList = outPanelGroupList };
        }

        [ReducerMethod]
        public static PanelsState ReduceDisposePanelGroupAction(
            PanelsState inState,
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
        public static PanelsState ReduceRegisterPanelTabAction(
            PanelsState inState,
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
        public static PanelsState ReduceDisposePanelTabAction(
            PanelsState inState,
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
        public static PanelsState ReduceSetActivePanelTabAction(
            PanelsState inState,
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
        public static PanelsState ReduceSetPanelTabAsActiveByContextRecordKeyAction(
            PanelsState inState,
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
        public static PanelsState ReduceSetDragEventArgsAction(
            PanelsState inState,
            SetDragEventArgsAction setDragEventArgsAction)
        {
            return inState with
            {
                DragEventArgs = setDragEventArgsAction.DragEventArgs
            };
        }
    }
}