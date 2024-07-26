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

    private bool _disposedValue;
    private ContextBoundary? _notificationContextBoundary;
    
    private Task HandleOnFocusIn(INotification notification)
    {
    	var localNotificationContextBoundary = _notificationContextBoundary;
    	
    	if (localNotificationContextBoundary is not null)
	    	localNotificationContextBoundary.HandleOnFocusIn();
	    	
	    return Task.CompletedTask;
    }
    
    private Task HandleOnFocusOut(INotification notification)
    {
    	return Task.CompletedTask;
    }
    
    protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (!_disposedValue)
        {
            if (disposing)
            {
                var notificationState = NotificationStateWrap.Value;

	            foreach (var notification in notificationState.DefaultList)
	            {
	                Dispatcher.Dispatch(new NotificationState.DisposeAction(notification.DynamicViewModelKey));
	            }
            }

            _disposedValue = true;
        }

        return base.DisposeAsyncCore(disposing);
	}
}