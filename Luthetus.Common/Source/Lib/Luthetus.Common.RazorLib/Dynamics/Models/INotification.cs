using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dynamics.Models;

public interface INotification : IDynamicViewModel
{
    public TimeSpan? NotificationOverlayLifespan { get; }
    public bool DeleteNotificationAfterOverlayIsDismissed { get; }
}
