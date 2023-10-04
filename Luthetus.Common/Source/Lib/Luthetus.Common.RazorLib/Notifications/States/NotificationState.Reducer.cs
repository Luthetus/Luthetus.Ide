using Fluxor;
using Luthetus.Common.RazorLib.Notifications.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Notifications.States;

public partial record NotificationState
{
    private class Reducer
    {
        [ReducerMethod]
        public static NotificationState ReduceRegisterAction(
            NotificationState inState,
            RegisterAction registerAction)
        {
            var outDefaultBag = inState.DefaultBag.Add(registerAction.Notification);

            return inState with { DefaultBag = outDefaultBag };
        }

        [ReducerMethod]
        public static NotificationState ReduceDisposeAction(
            NotificationState inState,
            DisposeAction disposeAction)
        {
            var inNotification = inState.DefaultBag.FirstOrDefault(
                x => x.Key == disposeAction.Key);

            if (inNotification is null)
                return inState;

            var outDefaultBag = inState.DefaultBag.Remove(inNotification);

            return inState with { DefaultBag = outDefaultBag };
        }

        [ReducerMethod]
        public static NotificationState ReduceMakeReadAction(
            NotificationState inState,
            MakeReadAction makeReadAction)
        {
            var inNotificationIndex = inState.DefaultBag.FindIndex(
                x => x.Key == makeReadAction.Key);

            if (inNotificationIndex == -1)
                return inState;

            var inNotification = inState.DefaultBag[inNotificationIndex];

            var outDefaultBag = inState.DefaultBag.RemoveAt(inNotificationIndex);
            var outReadBag = inState.ReadBag.Add(inNotification);

            return inState with
            {
                DefaultBag = outDefaultBag,
                ReadBag = outReadBag
            };
        }
        
        [ReducerMethod]
        public static NotificationState ReduceUndoMakeReadAction(
            NotificationState inState,
            UndoMakeReadAction undoMakeReadAction)
        {
            var inNotificationIndex = inState.DefaultBag.FindIndex(
                x => x.Key == undoMakeReadAction.Key);

            if (inNotificationIndex == -1)
                return inState;

            var inNotification = inState.DefaultBag[inNotificationIndex];

            var outReadBag = inState.ReadBag.RemoveAt(inNotificationIndex);
            var outDefaultBag = inState.DefaultBag.Add(inNotification);

            return inState with
            {
                DefaultBag = outDefaultBag,
                ReadBag = outReadBag
            };
        }

        [ReducerMethod]
        public static NotificationState ReduceMakeDeletedAction(
            NotificationState inState,
            MakeDeletedAction makeDeletedAction)
        {
            var inNotificationIndex = inState.DefaultBag.FindIndex(
                x => x.Key == makeDeletedAction.Key);

            if (inNotificationIndex == -1)
                return inState;

            var inNotification = inState.DefaultBag[inNotificationIndex];

            var outDefaultBag = inState.DefaultBag.RemoveAt(inNotificationIndex);
            var outDeletedBag = inState.DeletedBag.Add(inNotification);

            return inState with
            {
                DefaultBag = outDefaultBag,
                DeletedBag = outDeletedBag
            };
        }

        [ReducerMethod]
        public static NotificationState ReduceUndoMakeDeletedAction(
            NotificationState inState,
            UndoMakeDeletedAction undoMakeDeletedAction)
        {
            var inNotificationIndex = inState.DefaultBag.FindIndex(
                x => x.Key == undoMakeDeletedAction.Key);

            if (inNotificationIndex == -1)
                return inState;

            var inNotification = inState.DefaultBag[inNotificationIndex];

            var outDeletedBag = inState.DeletedBag.RemoveAt(inNotificationIndex);
            var outDefaultBag = inState.DefaultBag.Add(inNotification);

            return inState with
            {
                DefaultBag = outDefaultBag,
                DeletedBag = outDeletedBag
            };
        }

        [ReducerMethod]
        public static NotificationState ReduceMakeArchivedAction(
            NotificationState inState,
            MakeArchivedAction makeArchivedAction)
        {
            var inNotificationIndex = inState.DefaultBag.FindIndex(
                x => x.Key == makeArchivedAction.Key);

            if (inNotificationIndex == -1)
                return inState;

            var inNotification = inState.DefaultBag[inNotificationIndex];

            var outDefaultBag = inState.DefaultBag.RemoveAt(inNotificationIndex);
            var outArchivedBag = inState.ArchivedBag.Add(inNotification);

            return inState with
            {
                DefaultBag = outDefaultBag,
                ArchivedBag = outArchivedBag
            };
        }
        
        [ReducerMethod]
        public static NotificationState ReduceUndoMakeArchivedAction(
            NotificationState inState,
            UndoMakeArchivedAction undoMakeArchivedAction)
        {
            var inNotificationIndex = inState.DefaultBag.FindIndex(
                x => x.Key == undoMakeArchivedAction.Key);

            if (inNotificationIndex == -1)
                return inState;

            var inNotification = inState.DefaultBag[inNotificationIndex];

            var outArchivedBag = inState.ArchivedBag.RemoveAt(inNotificationIndex);
            var outDefaultBag = inState.DefaultBag.Add(inNotification);

            return inState with
            {
                DefaultBag = outDefaultBag,
                ArchivedBag = outArchivedBag
            };
        }

        [ReducerMethod(typeof(ClearDefaultAction))]
        public static NotificationState ReduceClearDefaultAction(
            NotificationState inState)
        {
            return inState with
            {
                DefaultBag = ImmutableList<NotificationRecord>.Empty
            };
        }
        
        [ReducerMethod(typeof(ClearReadAction))]
        public static NotificationState ReduceClearReadAction(
            NotificationState inState)
        {
            return inState with
            {
                ReadBag = ImmutableList<NotificationRecord>.Empty
            };
        }
        
        [ReducerMethod(typeof(ClearDeletedAction))]
        public static NotificationState ReduceClearDeletedAction(
            NotificationState inState)
        {
            return inState with
            {
                DeletedBag = ImmutableList<NotificationRecord>.Empty
            };
        }

        [ReducerMethod(typeof(ClearArchivedAction))]
        public static NotificationState ReduceClearArchivedAction(
            NotificationState inState)
        {
            return inState with
            {
                ArchivedBag = ImmutableList<NotificationRecord>.Empty
            };
        }
    }
}