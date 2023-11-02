using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;

namespace Luthetus.Common.RazorLib.Notifications.States;

public partial record NotificationStateTests
{
    public record RegisterAction(NotificationRecord Notification);
    public record DisposeAction(Key<NotificationRecord> Key);
    public record MakeReadAction(Key<NotificationRecord> Key);
    public record UndoMakeReadAction(Key<NotificationRecord> Key);
    public record MakeDeletedAction(Key<NotificationRecord> Key);
    public record UndoMakeDeletedAction(Key<NotificationRecord> Key);
    public record MakeArchivedAction(Key<NotificationRecord> Key);
    public record UndoMakeArchivedAction(Key<NotificationRecord> Key);
    public record ClearDefaultAction();
    public record ClearReadAction();
    public record ClearDeletedAction();
    public record ClearArchivedAction();
}