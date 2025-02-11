using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Notifications.Displays;

public partial class NotificationsViewDisplay : ComponentBase, IDisposable
{
    [Inject]
    private INotificationService NotificationService { get; set; } = null!;
    
    private readonly List<INotification> _emptyEntriesToRenderList = new();
    private readonly Action _defaultClearAction = new Action(() => { });

    private NotificationsViewKind _chosenNotificationsViewKind = NotificationsViewKind.Notifications;

	protected override void OnInitialized()
    {
    	NotificationService.NotificationStateChanged += OnNotificationStateChanged;
    	base.OnInitialized();
    }

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
        NotificationService.ReduceClearDefaultAction();
    }

    private void ClearRead()
    {
        NotificationService.ReduceClearReadAction();
    }

    private void ClearDeleted()
    {
        NotificationService.ReduceClearDeletedAction();
    }

    private void ClearArchived()
    {
        NotificationService.ReduceClearArchivedAction();
    }
    
    public async void OnNotificationStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
	
	public void Dispose()
	{
		NotificationService.NotificationStateChanged -= OnNotificationStateChanged;
	}
}
