namespace Luthetus.Common.RazorLib.PolymorphicViewModels.Models;

public interface INotificationViewModel
{
	public TimeSpan? NotificationOverlayLifespan { get; }
    public bool DeleteNotificationAfterOverlayIsDismissed { get; }
}

public class NotificationViewModel : INotificationViewModel
{
	public TimeSpan? NotificationOverlayLifespan { get; init; }
    public bool DeleteNotificationAfterOverlayIsDismissed { get; init; }
}
