using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Notifications.States;

public partial record NotificationState
{
    public record RegisterAction(INotification Notification);
    public record DisposeAction(Key<IDynamicViewModel> Key);
    public record MakeReadAction(Key<IDynamicViewModel> Key);
    public record UndoMakeReadAction(Key<IDynamicViewModel> Key);
    public record MakeDeletedAction(Key<IDynamicViewModel> Key);
    public record UndoMakeDeletedAction(Key<IDynamicViewModel> Key);
    public record MakeArchivedAction(Key<IDynamicViewModel> Key);
    public record UndoMakeArchivedAction(Key<IDynamicViewModel> Key);
    public record ClearDefaultAction();
    public record ClearReadAction();
    public record ClearDeletedAction();
    public record ClearArchivedAction();
}