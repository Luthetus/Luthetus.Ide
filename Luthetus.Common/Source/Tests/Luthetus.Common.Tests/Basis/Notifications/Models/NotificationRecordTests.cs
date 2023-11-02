using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Notifications.Models;

/// <summary>
/// If <see cref="NotificationOverlayLifespan"/> is null
/// then it will not be removed by the default timer based auto removal task.
/// </summary>
public record NotificationRecordTests(
    Key<NotificationRecord> Key,
    string Title,
    Type RendererType,
    Dictionary<string, object?>? ParameterMap,
    TimeSpan? NotificationOverlayLifespan,
    bool DeleteNotificationAfterOverlayIsDismissed,
    string? CssClassString);
