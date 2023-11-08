using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Notifications.States;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Notifications.States;

/// <summary>
/// <see cref="NotificationState"/>
/// </summary>
public class NotificationStateTests
{
    /// <summary>
    /// <see cref="NotificationState()"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var notificationState = new NotificationState();

        Assert.Equal(ImmutableList<NotificationRecord>.Empty, notificationState.DefaultBag);
        Assert.Equal(ImmutableList<NotificationRecord>.Empty, notificationState.ReadBag);
        Assert.Equal(ImmutableList<NotificationRecord>.Empty, notificationState.ArchivedBag);
        Assert.Equal(ImmutableList<NotificationRecord>.Empty, notificationState.DeletedBag);
    }
}