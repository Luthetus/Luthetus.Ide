namespace Luthetus.Common.RazorLib.Dynamics.Models;

public interface INotification : IDynamicViewModel
{
	public string? NotificationCssClass { get; set; }
	public string? NotificationCssStyle { get; set; }
	public TimeSpan? NotificationOverlayLifespan { get; }
    public bool DeleteNotificationAfterOverlayIsDismissed { get; }
}
