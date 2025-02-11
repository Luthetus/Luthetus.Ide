using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;

namespace Luthetus.Common.RazorLib.Notifications.Displays;

public partial class NotificationsInteractiveIconDisplay : ComponentBase, IDisposable
{
    [Inject]
    private INotificationService NotificationService { get; set; } = null!;
    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;
    
    private const string _buttonElementId = "luth_web_notifications-interactive-icon_id";

    private readonly DialogViewModel NotificationsViewDisplayDialogRecord = new(
        Key<IDynamicViewModel>.NewKey(),
        "Notifications",
        typeof(NotificationsViewDisplay),
        null,
        null,
		true,
		_buttonElementId);
		
	protected override void OnInitialized()
    {
    	NotificationService.NotificationStateChanged += OnNotificationStateChanged;
    	base.OnInitialized();
    }

    private void ShowNotificationsViewDisplayOnClick()
    {
        DialogService.ReduceRegisterAction(NotificationsViewDisplayDialogRecord);
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