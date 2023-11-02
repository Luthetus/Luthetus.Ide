using Fluxor;

namespace Luthetus.Common.RazorLib.Panels.States;

public partial record PanelsStateTests
{
    private class Reducer
    {
        [ReducerMethod]
        public static PanelsState ReduceRegisterPanelGroupAction(
            PanelsState inState,
            RegisterPanelGroupAction registerPanelGroupAction)
        {
            if (inState.PanelGroupBag.Any(x => x.Key == registerPanelGroupAction.PanelGroup.Key))
                return inState;

            var outPanelGroupBag = inState.PanelGroupBag.Add(registerPanelGroupAction.PanelGroup);

            return inState with { PanelGroupBag = outPanelGroupBag };
        }

        [ReducerMethod]
        public static PanelsState ReduceDisposePanelGroupAction(
            PanelsState inState,
            DisposePanelGroupAction disposePanelGroupAction)
        {
            var inPanelGroup = inState.PanelGroupBag.FirstOrDefault(
                x => x.Key == disposePanelGroupAction.PanelGroupKey);

            if (inPanelGroup is null)
                return inState;

            var outPanelGroupBag = inState.PanelGroupBag.Remove(inPanelGroup);

            return inState with { PanelGroupBag = outPanelGroupBag };
        }

        [ReducerMethod]
        public static PanelsState ReduceRegisterPanelTabAction(
            PanelsState inState,
            RegisterPanelTabAction registerPanelTabAction)
        {
            var inPanelGroup = inState.PanelGroupBag.FirstOrDefault(
                x => x.Key == registerPanelTabAction.PanelGroupKey);

            if (inPanelGroup is null)
                return inState;

            var outTabBag = inPanelGroup.TabBag;

            var insertionPoint = registerPanelTabAction.InsertAtIndexZero
                ? 0
                : inPanelGroup.TabBag.Length;

            outTabBag = inPanelGroup.TabBag.Insert(insertionPoint, registerPanelTabAction.PanelTab);

            var outPanelGroupBag = inState.PanelGroupBag.Replace(inPanelGroup, inPanelGroup with
            {
                TabBag = outTabBag
            });

            return inState with
            {
                PanelGroupBag = outPanelGroupBag
            };
        }

        [ReducerMethod]
        public static PanelsState ReduceDisposePanelTabAction(
            PanelsState inState,
            DisposePanelTabAction disposePanelTabAction)
        {
            var inPanelGroup = inState.PanelGroupBag.FirstOrDefault(
                x => x.Key == disposePanelTabAction.PanelGroupKey);

            if (inPanelGroup is null)
                return inState;

            var panelTab = inPanelGroup.TabBag.FirstOrDefault(
                x => x.Key == disposePanelTabAction.PanelTabKey);

            if (panelTab is null)
                return inState;

            var outTabBag = inPanelGroup.TabBag.Remove(panelTab);

            var outPanelGroupBag = inState.PanelGroupBag.Replace(inPanelGroup, inPanelGroup with
            {
                TabBag = outTabBag
            });

            return inState with { PanelGroupBag = outPanelGroupBag };
        }

        [ReducerMethod]
        public static PanelsState ReduceSetActivePanelTabAction(
            PanelsState inState,
            SetActivePanelTabAction setActivePanelTabAction)
        {
            var inPanelGroup = inState.PanelGroupBag.FirstOrDefault(
                x => x.Key == setActivePanelTabAction.PanelGroupKey);

            if (inPanelGroup is null)
                return inState;

            var outPanelGroupBag = inState.PanelGroupBag.Replace(inPanelGroup, inPanelGroup with
            {
                ActiveTabKey = setActivePanelTabAction.PanelTabKey
            });

            return inState with { PanelGroupBag = outPanelGroupBag };
        }

        [ReducerMethod]
        public static PanelsState ReduceSetPanelTabAsActiveByContextRecordKeyAction(
            PanelsState inState,
            SetPanelTabAsActiveByContextRecordKeyAction setPanelTabAsActiveByContextRecordKeyAction)
        {
            var inPanelGroup = inState.PanelGroupBag.FirstOrDefault(x => x.TabBag
                .Any(y => y.ContextRecordKey == setPanelTabAsActiveByContextRecordKeyAction.ContextRecordKey));

            if (inPanelGroup is null)
                return inState;

            var inPanelTab = inPanelGroup.TabBag.FirstOrDefault(
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