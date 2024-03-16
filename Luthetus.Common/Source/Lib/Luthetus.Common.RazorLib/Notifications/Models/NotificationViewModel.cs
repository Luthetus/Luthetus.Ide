using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Notifications.Models;

public class NotificationViewModel : INotificationViewModel
{
	public NotificationViewModel(
		Key<INotificationViewModel> key,
        string title,
        Type rendererType,
        Dictionary<string, object?>? parameterMap,
        TimeSpan? notificationOverlayLifespan,
        bool deleteNotificationAfterOverlayIsDismissed,
        string? cssClassString,
		IPolymorphicViewModel? polymorphicViewModel)
	{
		Key = key;
		Title = title;
		RendererType = rendererType;
		ParameterMap = parameterMap;
		NotificationOverlayLifespan = notificationOverlayLifespan;
		DeleteNotificationAfterOverlayIsDismissed = deleteNotificationAfterOverlayIsDismissed;
		CssClassString = cssClassString;
		PolymorphicViewModel = polymorphicViewModel;
	}
	
	public IPolymorphicViewModel? PolymorphicViewModel { get; init; }
	public Key<INotificationViewModel> Key { get; init; }
	public TimeSpan? NotificationOverlayLifespan { get; init; }
    public bool DeleteNotificationAfterOverlayIsDismissed { get; init; }
	public string? CssClassString { get; init; }
	public string Title { get; init; }
    public Type RendererType { get; init; }
    public Dictionary<string, object?>? ParameterMap { get; init; }
}
