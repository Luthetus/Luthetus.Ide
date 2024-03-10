namespace Luthetus.Common.RazorLib.PolymorphicUis.Models;

public interface IPolymorphicNotification : IPolymorphicUiRecord
{
    public TimeSpan? NotificationOverlayLifespan { get; }
    public bool DeleteNotificationAfterOverlayIsDismissed { get; }
}
