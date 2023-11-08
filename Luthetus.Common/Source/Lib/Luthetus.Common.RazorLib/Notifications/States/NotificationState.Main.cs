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
    ImmutableList<NotificationRecord> DefaultBag,
    ImmutableList<NotificationRecord> ReadBag,
    ImmutableList<NotificationRecord> ArchivedBag,
    ImmutableList<NotificationRecord> DeletedBag)
{
    public NotificationState() : this(
        ImmutableList<NotificationRecord>.Empty,
        ImmutableList<NotificationRecord>.Empty,
        ImmutableList<NotificationRecord>.Empty,
        ImmutableList<NotificationRecord>.Empty)
    {
        
    }
}