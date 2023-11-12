using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Notifications.States;

namespace Luthetus.Common.Tests.Basis.Notifications.States;

/// <summary>
/// <see cref="NotificationState"/>
/// </summary>
public class NotificationStateActionsTests
{
    /// <summary>
    /// <see cref="NotificationState.RegisterAction"/>
    /// </summary>
    [Fact]
    public void RegisterAction()
    {
        InitializeNotificationStateActionsTests(out var notificationRecord);

        var registerAction = new NotificationState.RegisterAction(notificationRecord);
        Assert.Equal(notificationRecord, registerAction.Notification);
    }

    /// <summary>
    /// <see cref="NotificationState.DisposeAction"/>
    /// </summary>
    [Fact]
    public void DisposeAction()
    {
	    throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="NotificationState.MakeReadAction"/>
    /// </summary>
    [Fact]
    public void MakeReadAction()
    {
	    throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="NotificationState.UndoMakeReadAction"/>
    /// </summary>
    [Fact]
    public void UndoMakeReadAction()
    {
	    throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="NotificationState.MakeDeletedAction"/>
    /// </summary>
    [Fact]
    public void MakeDeletedAction()
    {
	    throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="NotificationState.UndoMakeDeletedAction"/>
    /// </summary>
    [Fact]
    public void UndoMakeDeletedAction()
    {
	    throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="NotificationState.MakeArchivedAction"/>
    /// </summary>
    [Fact]
    public void MakeArchivedAction()
    {
	    throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="NotificationState.UndoMakeArchivedAction"/>
    /// </summary>
    [Fact]
    public void UndoMakeArchivedAction()
    {
	    throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="NotificationState.ClearDefaultAction"/>
    /// </summary>
    [Fact]
    public void ClearDefaultAction()
    {
	    throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="NotificationState.ClearReadAction"/>
    /// </summary>
    [Fact]
    public void ClearReadAction()
    {
	    throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="NotificationState.ClearDeletedAction"/>
    /// </summary>
    [Fact]
    public void ClearDeletedAction()
    {
	    throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="NotificationState.ClearArchivedAction"/>
    /// </summary>
    [Fact]
    public void ClearArchivedAction()
    {
        throw new NotImplementedException();
    }

    private void InitializeNotificationStateActionsTests(
        out NotificationRecord sampleNotificationRecord)
    {
        sampleNotificationRecord = new NotificationRecord(
            Key<NotificationRecord>.NewKey(),
            "Test title",
            typeof(CommonInformativeNotificationDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CommonInformativeNotificationDisplay.Message),
                    "Test message"
                }
            },
            null,
            true,
            null);
    }
}