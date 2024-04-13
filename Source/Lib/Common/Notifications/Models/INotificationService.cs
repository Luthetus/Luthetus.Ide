using Fluxor;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.States;

namespace Luthetus.Common.RazorLib.Notifications.Models;

/// <summary>
/// TODO: SphagettiCode - Not all NotificationState Actions are mapped to a public method (2023-09-19)
/// on this service. Furthermore, NotificationState itself is sphagetti code
/// </summary>
public interface INotificationService
{
    public IState<NotificationState> NotificationStateWrap { get; }

    public void RegisterNotificationRecord(INotification notification);
    public void DisposeNotificationRecord(Key<IDynamicViewModel> dynamicViewModelKey);
}