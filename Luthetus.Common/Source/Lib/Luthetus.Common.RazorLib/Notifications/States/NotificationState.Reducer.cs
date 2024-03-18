using Fluxor;
using Luthetus.Common.RazorLib.Dynamics.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Notifications.States;

public partial record NotificationState
{
    public class Reducer
    {
        [ReducerMethod]
        public static NotificationState ReduceRegisterAction(
            NotificationState inState,
            RegisterAction registerAction)
        {
            var outDefaultList = inState.DefaultList.Add(registerAction.Notification);

            return inState with { DefaultList = outDefaultList };
        }

        [ReducerMethod]
        public static NotificationState ReduceDisposeAction(
            NotificationState inState,
            DisposeAction disposeAction)
        {
            var inNotification = inState.DefaultList.FirstOrDefault(
                x => x.DynamicViewModelKey == disposeAction.Key);

            if (inNotification is null)
                return inState;

            var outDefaultList = inState.DefaultList.Remove(inNotification);

            return inState with { DefaultList = outDefaultList };
        }

        [ReducerMethod]
        public static NotificationState ReduceMakeReadAction(
            NotificationState inState,
            MakeReadAction makeReadAction)
        {
            var inNotificationIndex = inState.DefaultList.FindIndex(
                x => x.DynamicViewModelKey == makeReadAction.Key);

            if (inNotificationIndex == -1)
                return inState;

            var inNotification = inState.DefaultList[inNotificationIndex];

            var outDefaultList = inState.DefaultList.RemoveAt(inNotificationIndex);
            var outReadList = inState.ReadList.Add(inNotification);

            return inState with
            {
                DefaultList = outDefaultList,
                ReadList = outReadList
            };
        }
        
        [ReducerMethod]
        public static NotificationState ReduceUndoMakeReadAction(
            NotificationState inState,
            UndoMakeReadAction undoMakeReadAction)
        {
            var inNotificationIndex = inState.ReadList.FindIndex(
                x => x.DynamicViewModelKey == undoMakeReadAction.Key);

            if (inNotificationIndex == -1)
                return inState;

            var inNotification = inState.ReadList[inNotificationIndex];

            var outReadList = inState.ReadList.RemoveAt(inNotificationIndex);
            var outDefaultList = inState.DefaultList.Add(inNotification);

            return inState with
            {
                DefaultList = outDefaultList,
                ReadList = outReadList
            };
        }

        [ReducerMethod]
        public static NotificationState ReduceMakeDeletedAction(
            NotificationState inState,
            MakeDeletedAction makeDeletedAction)
        {
            var inNotificationIndex = inState.DefaultList.FindIndex(
                x => x.DynamicViewModelKey == makeDeletedAction.Key);

            if (inNotificationIndex == -1)
                return inState;

            var inNotification = inState.DefaultList[inNotificationIndex];

            var outDefaultList = inState.DefaultList.RemoveAt(inNotificationIndex);
            var outDeletedList = inState.DeletedList.Add(inNotification);

            return inState with
            {
                DefaultList = outDefaultList,
                DeletedList = outDeletedList
            };
        }

        [ReducerMethod]
        public static NotificationState ReduceUndoMakeDeletedAction(
            NotificationState inState,
            UndoMakeDeletedAction undoMakeDeletedAction)
        {
            var inNotificationIndex = inState.DeletedList.FindIndex(
                x => x.DynamicViewModelKey == undoMakeDeletedAction.Key);

            if (inNotificationIndex == -1)
                return inState;

            var inNotification = inState.DeletedList[inNotificationIndex];

            var outDeletedList = inState.DeletedList.RemoveAt(inNotificationIndex);
            var outDefaultList = inState.DefaultList.Add(inNotification);

            return inState with
            {
                DefaultList = outDefaultList,
                DeletedList = outDeletedList
            };
        }

        [ReducerMethod]
        public static NotificationState ReduceMakeArchivedAction(
            NotificationState inState,
            MakeArchivedAction makeArchivedAction)
        {
            var inNotificationIndex = inState.DefaultList.FindIndex(
                x => x.DynamicViewModelKey == makeArchivedAction.Key);

            if (inNotificationIndex == -1)
                return inState;

            var inNotification = inState.DefaultList[inNotificationIndex];

            var outDefaultList = inState.DefaultList.RemoveAt(inNotificationIndex);
            var outArchivedList = inState.ArchivedList.Add(inNotification);

            return inState with
            {
                DefaultList = outDefaultList,
                ArchivedList = outArchivedList
            };
        }
        
        [ReducerMethod]
        public static NotificationState ReduceUndoMakeArchivedAction(
            NotificationState inState,
            UndoMakeArchivedAction undoMakeArchivedAction)
        {
            var inNotificationIndex = inState.ArchivedList.FindIndex(
                x => x.DynamicViewModelKey == undoMakeArchivedAction.Key);

            if (inNotificationIndex == -1)
                return inState;

            var inNotification = inState.ArchivedList[inNotificationIndex];

            var outArchivedList = inState.ArchivedList.RemoveAt(inNotificationIndex);
            var outDefaultList = inState.DefaultList.Add(inNotification);

            return inState with
            {
                DefaultList = outDefaultList,
                ArchivedList = outArchivedList
            };
        }

        [ReducerMethod(typeof(ClearDefaultAction))]
        public static NotificationState ReduceClearDefaultAction(
            NotificationState inState)
        {
            return inState with
            {
                DefaultList = ImmutableList<INotification>.Empty
            };
        }
        
        [ReducerMethod(typeof(ClearReadAction))]
        public static NotificationState ReduceClearReadAction(
            NotificationState inState)
        {
            return inState with
            {
                ReadList = ImmutableList<INotification>.Empty
            };
        }
        
        [ReducerMethod(typeof(ClearDeletedAction))]
        public static NotificationState ReduceClearDeletedAction(
            NotificationState inState)
        {
            return inState with
            {
                DeletedList = ImmutableList<INotification>.Empty
            };
        }

        [ReducerMethod(typeof(ClearArchivedAction))]
        public static NotificationState ReduceClearArchivedAction(
            NotificationState inState)
        {
            return inState with
            {
                ArchivedList = ImmutableList<INotification>.Empty
            };
        }
    }
}