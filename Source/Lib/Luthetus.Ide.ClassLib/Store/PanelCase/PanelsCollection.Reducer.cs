using Fluxor;

namespace Luthetus.Ide.ClassLib.Store.PanelCase;

public partial record PanelsCollection
{
    private class Reducer
    {
        [ReducerMethod]
        public static PanelsCollection ReduceRegisterPanelRecordAction(
            PanelsCollection previousPanelsCollection,
            RegisterPanelRecordAction registerPanelRecordAction)
        {
            if (previousPanelsCollection.PanelRecordsList.Any(
                    x => x.PanelRecordKey == registerPanelRecordAction.PanelRecord.PanelRecordKey))
            {
                return previousPanelsCollection;
            }

            var nextPanelsList = previousPanelsCollection.PanelRecordsList.Add(
                registerPanelRecordAction.PanelRecord);

            return previousPanelsCollection with
            {
                PanelRecordsList = nextPanelsList
            };
        }

        [ReducerMethod]
        public static PanelsCollection ReduceDisposePanelRecordAction(
            PanelsCollection previousPanelsCollection,
            DisposePanelRecordAction disposePanelRecordAction)
        {
            var targetedRecordModel = previousPanelsCollection.PanelRecordsList.FirstOrDefault(
                    x => x.PanelRecordKey == disposePanelRecordAction.PanelRecordKey);

            if (targetedRecordModel is null)
                return previousPanelsCollection;

            var nextPanelsList = previousPanelsCollection.PanelRecordsList.Remove(
                targetedRecordModel);

            return previousPanelsCollection with
            {
                PanelRecordsList = nextPanelsList
            };
        }

        [ReducerMethod]
        public static PanelsCollection ReduceRegisterPanelTabAction(
            PanelsCollection previousPanelsCollection,
            RegisterPanelTabAction registerPanelTabAction)
        {
            var targetedPanelRecord = previousPanelsCollection.PanelRecordsList.FirstOrDefault(
                    x => x.PanelRecordKey == registerPanelTabAction.PanelRecordKey);

            if (targetedPanelRecord is null)
                return previousPanelsCollection;

            var nextPanelTabs = targetedPanelRecord.PanelTabs.Add(
                registerPanelTabAction.PanelTab);

            var nextPanelRecord = targetedPanelRecord with
            {
                PanelTabs = nextPanelTabs
            };

            var nextPanelsList = previousPanelsCollection.PanelRecordsList.Replace(
                targetedPanelRecord,
                nextPanelRecord);

            return previousPanelsCollection with
            {
                PanelRecordsList = nextPanelsList
            };
        }

        [ReducerMethod]
        public static PanelsCollection ReduceDisposePanelTabAction(
            PanelsCollection previousPanelsCollection,
            DisposePanelTabAction disposePanelTabAction)
        {
            var targetedPanelRecord = previousPanelsCollection.PanelRecordsList.FirstOrDefault(
                    x => x.PanelRecordKey == disposePanelTabAction.PanelRecordKey);

            if (targetedPanelRecord is null)
                return previousPanelsCollection;

            var panelTabToRemove = targetedPanelRecord.PanelTabs.FirstOrDefault(
                x => x.PanelTabKey == disposePanelTabAction.PanelTabKey);

            if (panelTabToRemove is null)
                return previousPanelsCollection;

            var nextPanelTabs = targetedPanelRecord.PanelTabs.Remove(
                panelTabToRemove);

            var nextPanelRecord = targetedPanelRecord with
            {
                PanelTabs = nextPanelTabs
            };

            var nextPanelsList = previousPanelsCollection.PanelRecordsList.Replace(
                targetedPanelRecord,
                nextPanelRecord);

            return previousPanelsCollection with
            {
                PanelRecordsList = nextPanelsList
            };
        }

        [ReducerMethod]
        public static PanelsCollection ReduceSetActivePanelTabAction(
            PanelsCollection previousPanelsCollection,
            SetActivePanelTabAction setActivePanelTabAction)
        {
            var targetedPanelRecord = previousPanelsCollection.PanelRecordsList.FirstOrDefault(
                    x => x.PanelRecordKey == setActivePanelTabAction.PanelRecordKey);

            if (targetedPanelRecord is null)
                return previousPanelsCollection;

            var nextPanelRecord = targetedPanelRecord with
            {
                ActivePanelTabKey = setActivePanelTabAction.PanelTabKey
            };

            var nextPanelsList = previousPanelsCollection.PanelRecordsList.Replace(
                targetedPanelRecord,
                nextPanelRecord);

            return previousPanelsCollection with
            {
                PanelRecordsList = nextPanelsList
            };
        }

        [ReducerMethod]
        public static PanelsCollection ReduceSetPanelDragEventArgsAction(
            PanelsCollection previousPanelsCollection,
            SetPanelDragEventArgsAction setPanelDragEventArgsAction)
        {
            return previousPanelsCollection with
            {
                PanelDragEventArgs = setPanelDragEventArgsAction.PanelDragEventArgs
            };
        }
    }
}