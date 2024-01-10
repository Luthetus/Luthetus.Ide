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

        Assert.Equal(ImmutableList<NotificationRecord>.Empty, notificationState.DefaultList);
        Assert.Equal(ImmutableList<NotificationRecord>.Empty, notificationState.ReadList);
        Assert.Equal(ImmutableList<NotificationRecord>.Empty, notificationState.ArchivedList);
        Assert.Equal(ImmutableList<NotificationRecord>.Empty, notificationState.DeletedList);
    }
}