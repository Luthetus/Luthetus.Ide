namespace Luthetus.Common.RazorLib.PolymorphicViewModels.Models;

public class NotificationViewModel : INotificationViewModel
{
	public TimeSpan? NotificationOverlayLifespan { get; init; }
    public bool DeleteNotificationAfterOverlayIsDismissed { get; init; }
}
