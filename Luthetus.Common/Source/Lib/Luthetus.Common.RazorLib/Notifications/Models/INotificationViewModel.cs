using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Notifications.Models;

public interface INotificationViewModel
{
	public IPolymorphicViewModel? PolymorphicViewModel { get; }
	public Key<INotificationViewModel> Key { get; }
	public TimeSpan? NotificationOverlayLifespan { get; }
    public bool DeleteNotificationAfterOverlayIsDismissed { get; }
	public string? CssClassString { get; }
	public string Title { get; }
    public Type RendererType { get; }
    public Dictionary<string, object?>? ParameterMap { get; }
}
