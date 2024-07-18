using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
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
        Assert.Equal(dialogRecord, registerAction.Dialog);
    }

    /// <summary>
    /// <see cref="DialogState.DisposeAction"/>
    /// </summary>
    [Fact]
    public void DisposeAction()
    {
        InitializeDialogStateActionsTests(out var dialogRecord);

        var disposeAction = new DialogState.DisposeAction(dialogRecord.DynamicViewModelKey);
        Assert.Equal(dialogRecord.DynamicViewModelKey, disposeAction.DynamicViewModelKey);
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
            var setIsMaximizedAction = new DialogState.SetIsMaximizedAction(dialogRecord.DynamicViewModelKey, true);
            Assert.Equal(dialogRecord.DynamicViewModelKey, setIsMaximizedAction.DynamicViewModelKey);
            Assert.True(setIsMaximizedAction.IsMaximized);
        }
        
        // false
        {
            var setIsMaximizedAction = new DialogState.SetIsMaximizedAction(dialogRecord.DynamicViewModelKey, false);
            Assert.Equal(dialogRecord.DynamicViewModelKey, setIsMaximizedAction.DynamicViewModelKey);
            Assert.False(setIsMaximizedAction.IsMaximized);
        }
    }

    private void InitializeDialogStateActionsTests(out IDialog sampleDialogRecord)
    {
        sampleDialogRecord = new DialogViewModel(Key<IDynamicViewModel>.NewKey(), "Test title",
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