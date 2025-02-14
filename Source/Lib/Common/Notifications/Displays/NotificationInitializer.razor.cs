using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Contexts.Displays;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Notifications.Displays;

public partial class NotificationInitializer : ComponentBase, IDisposable
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;

    private ContextBoundary? _notificationContextBoundary;
    
    protected override void OnInitialized()
    {
		CommonApi.NotificationApi.NotificationStateChanged += OnNotificationStateChanged;
    	base.OnInitialized();
    }
    
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
    
    public async void OnNotificationStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
	
	public void Dispose()
	{
		CommonApi.NotificationApi.NotificationStateChanged -= OnNotificationStateChanged;
	
		var notificationState = CommonApi.NotificationApi.GetNotificationState();

        foreach (var notification in notificationState.DefaultList)
        {
            CommonApi.NotificationApi.ReduceDisposeAction(notification.DynamicViewModelKey);
        }
	}
}
