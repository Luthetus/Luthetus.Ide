using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;

namespace Luthetus.Common.Tests.Basis.Dialogs.States;

/// <summary>
/// <see cref="DialogState"/>
/// </summary>
public class DialogStateActionsTests
{
    /// <summary>
    /// <see cref="DialogState.RegisterAction"/>
    /// </summary>
    [Fact]
    public void RegisterAction()
    {
        InitializeDialogStateActionsTests(out var dialogRecord);

        var registerAction = new DialogState.RegisterAction(dialogRecord);
        Assert.Equal(dialogRecord, registerAction.Entry);
    }

    /// <summary>
    /// <see cref="DialogState.DisposeAction"/>
    /// </summary>
    [Fact]
    public void DisposeAction()
    {
        InitializeDialogStateActionsTests(out var dialogRecord);

        var disposeAction = new DialogState.DisposeAction(dialogRecord.Key);
        Assert.Equal(dialogRecord.Key, disposeAction.Key);
    }

    /// <summary>
    /// <see cref="DialogState.SetIsMaximizedAction"/>
    /// </summary>
    [Fact]
    public void SetIsMaximizedAction()
    {
        InitializeDialogStateActionsTests(out var dialogRecord);

        // true
        {
            var setIsMaximizedAction = new DialogState.SetIsMaximizedAction(dialogRecord.Key, true);
            Assert.Equal(dialogRecord.Key, setIsMaximizedAction.Key);
            Assert.True(setIsMaximizedAction.IsMaximized);
        }
        
        // false
        {
            var setIsMaximizedAction = new DialogState.SetIsMaximizedAction(dialogRecord.Key, false);
            Assert.Equal(dialogRecord.Key, setIsMaximizedAction.Key);
            Assert.False(setIsMaximizedAction.IsMaximized);
        }
    }

    private void InitializeDialogStateActionsTests(
        out DialogRecord sampleDialogRecord)
    {
        sampleDialogRecord = new DialogRecord(Key<DialogRecord>.NewKey(), "Test title",
            typeof(CommonInformativeNotificationDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CommonInformativeNotificationDisplay.Message),
                    "Test message"
                }
            },
            null);
    }
}