using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Notifications.States;
using Luthetus.Common.RazorLib.Contexts.Displays;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Notifications.Displays;

public partial class NotificationInitializer : FluxorComponent
{
    [Inject]
    private IState<NotificationState> NotificationStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private bool _disposed;
    private ContextBoundary? _notificationContextBoundary;
    
    private Task HandleOnFocusIn(INotification notification)
    {
    	var localNotificationContextBoundary = _notificationContextBoundary;
    	
    	if (localNotificationContextBoundary is not null)
	    	localNotificationContextBoundary.HandleOnFocusIn(shouldShowOutline: false);
	    	
	    return Task.CompletedTask;
    }
    
    private Task HandleOnFocusOut(INotification notification)
    {
    	var localNotificationContextBoundary = _notificationContextBoundary;
    	
    	if (localNotificationContextBoundary is not null)
	    	localNotificationContextBoundary.HandleOnFocusOut();
	    	
		return Task.CompletedTask;
    }

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