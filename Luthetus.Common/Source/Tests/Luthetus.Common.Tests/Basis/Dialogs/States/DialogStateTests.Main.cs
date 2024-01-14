using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Dialogs.States;

/// <summary>
/// <see cref="DialogState"/>
/// </summary>
public class DialogStateMainTests
{
    /// <summary>
    /// <see cref="DialogState()"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var dialogState = new DialogState();
        Assert.Equal(ImmutableList<DialogRecord>.Empty, dialogState.DialogList);
    }

    /// <summary>
    /// <see cref="DialogState.DialogList"/>
    /// </summary>
    [Fact]
    public void DialogList()
    {
        var dialogState = new DialogState();
        Assert.Equal(ImmutableList<DialogRecord>.Empty, dialogState.DialogList);

        var sampleDialogRecord = new DialogRecord(Key<DialogRecord>.NewKey(), "Test title",
            typeof(CommonInformativeNotificationDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CommonInformativeNotificationDisplay.Message),
                    "Test message"
                }
            },
            null);

        var outDialogList = dialogState.DialogList.Add(sampleDialogRecord);

        dialogState = dialogState with
        {
            DialogList = outDialogList
        };

        Assert.Contains(dialogState.DialogList, x => x == sampleDialogRecord);
    }
}