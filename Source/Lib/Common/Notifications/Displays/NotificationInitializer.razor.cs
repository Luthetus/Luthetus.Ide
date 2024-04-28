using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Notifications.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Notifications.Displays;

public partial class NotificationInitializer : FluxorComponent
{
    [Inject]
    private IState<NotificationState> NotificationStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private bool _disposed;

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _disposed = true;

            var notificationState = NotificationStateWrap.Value;

            foreach (var notification in notificationState.DefaultList)
            {
                Dispatcher.Dispatch(new NotificationState.DisposeAction(notification.DynamicViewModelKey));
            }
        }

        base.Dispose(disposing);
    }
}