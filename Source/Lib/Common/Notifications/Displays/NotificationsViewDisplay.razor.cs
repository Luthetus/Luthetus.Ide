using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Notifications.States;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Notifications.Displays;

public partial class NotificationsViewDisplay : FluxorComponent
{
    [Inject]
    private IState<NotificationState> NotificationStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    private readonly List<INotification> _emptyEntriesToRenderList = new();
    private readonly Action _defaultClearAction = new Action(() => { });

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
