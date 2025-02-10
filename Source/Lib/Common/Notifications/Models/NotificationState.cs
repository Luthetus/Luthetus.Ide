using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Notifications.Models;

/// <summary>
/// The list provided should not be modified after passing it as a parameter.
/// Make a shallow copy, and pass the shallow copy, if further modification of your list will be necessary.
/// 
/// ---
/// 
/// TODO: SphagettiCode - The NotificationState is written such that there are (2023-09-19)
/// 4 lists. One foreach filter option. And the NotificationRecord gets shuffled around.
/// This is odd. Perhaps use one list and filter it?
/// </summary>
public record NotificationState(
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