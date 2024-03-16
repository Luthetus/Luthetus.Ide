using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;

namespace Luthetus.Common.RazorLib.Notifications.States;

public partial record NotificationState
{
    public record RegisterAction(INotificationViewModel Notification);
    public record DisposeAction(Key<INotificationViewModel> Key);
    public record MakeReadAction(Key<INotificationViewModel> Key);
    public record UndoMakeReadAction(Key<INotificationViewModel> Key);
    public record MakeDeletedAction(Key<INotificationViewModel> Key);
    public record UndoMakeDeletedAction(Key<INotificationViewModel> Key);
    public record MakeArchivedAction(Key<INotificationViewModel> Key);
    public record UndoMakeArchivedAction(Key<INotificationViewModel> Key);
    public record ClearDefaultAction();
    public record ClearReadAction();
    public record ClearDeletedAction();
    public record ClearArchivedAction();
}