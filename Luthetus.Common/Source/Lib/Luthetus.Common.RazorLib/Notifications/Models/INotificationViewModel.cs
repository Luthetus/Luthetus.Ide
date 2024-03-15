namespace Luthetus.Common.RazorLib.Notifications.Models;

public interface INotificationViewModel
{
	public TimeSpan? NotificationOverlayLifespan { get; }
    public bool DeleteNotificationAfterOverlayIsDismissed { get; }
}
