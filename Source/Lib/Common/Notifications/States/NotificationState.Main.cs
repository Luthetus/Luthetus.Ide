using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Notifications.States;

/// <summary>
/// TODO: SphagettiCode - The NotificationState is written such that there are (2023-09-19)
/// 4 lists. One foreach filter option. And the NotificationRecord gets shuffled around.
/// This is odd. Perhaps use one list and filter it?
/// </summary>
[FeatureState]
public partial record NotificationState(
    List<INotification> DefaultList,
    List<INotification> ReadList,
    List<INotification> ArchivedList,
    List<INotification> DeletedList)
{
    public NotificationState() : this(
        new List<INotification>(),
        new List<INotification>(),
        new List<INotification>(),
        new List<INotification>())
    {
        
    }
}