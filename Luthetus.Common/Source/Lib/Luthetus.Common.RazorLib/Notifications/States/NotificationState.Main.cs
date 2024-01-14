using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Notifications.Models;

namespace Luthetus.Common.RazorLib.Notifications.States;

/// <summary>
/// TODO: SphagettiCode - The NotificationState is written such that there are (2023-09-19)
/// 4 lists. One foreach filter option. And the NotificationRecord gets shuffled around.
/// This is odd. Perhaps use one list and filter it?
/// </summary>
[FeatureState]
public partial record NotificationState(
    ImmutableList<NotificationRecord> DefaultList,
    ImmutableList<NotificationRecord> ReadList,
    ImmutableList<NotificationRecord> ArchivedList,
    ImmutableList<NotificationRecord> DeletedList)
{
    public NotificationState() : this(
        ImmutableList<NotificationRecord>.Empty,
        ImmutableList<NotificationRecord>.Empty,
        ImmutableList<NotificationRecord>.Empty,
        ImmutableList<NotificationRecord>.Empty)
    {
        
    }
}