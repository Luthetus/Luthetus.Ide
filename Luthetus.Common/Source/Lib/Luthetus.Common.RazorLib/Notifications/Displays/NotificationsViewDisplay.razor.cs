using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Notifications.States;
using Luthetus.Common.RazorLib.Notifications.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Notifications.Displays;

public partial class NotificationsViewDisplay : FluxorComponent
{
    [Inject]
    private IState<NotificationState> NotificationStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private NotificationsViewKind _chosenNotificationsViewKind = NotificationsViewKind.Notifications;

    private string GetIsActiveCssClass(
        NotificationsViewKind chosenNotificationsViewKind,
        NotificationsViewKind iterationNotificationsViewKind)
    {
        return chosenNotificationsViewKind == iterationNotificationsViewKind
            ? "luth_active"
            : string.Empty;
    }

    private void Clear()
    {
        Dispatcher.Dispatch(new NotificationState.ClearDefaultAction());
    }

    private void ClearRead()
    {
        Dispatcher.Dispatch(new NotificationState.ClearReadAction());
    }

    private void ClearDeleted()
    {
        Dispatcher.Dispatch(new NotificationState.ClearDeletedAction());
    }

    private void ClearArchived()
    {
        Dispatcher.Dispatch(new NotificationState.ClearArchivedAction());
    }
}
