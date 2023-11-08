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
        Assert.Equal(ImmutableList<DialogRecord>.Empty, dialogState.DialogBag);
    }

    /// <summary>
    /// <see cref="DialogState.DialogBag"/>
    /// </summary>
    [Fact]
    public void DialogBag()
    {
        var dialogState = new DialogState();
        Assert.Equal(ImmutableList<DialogRecord>.Empty, dialogState.DialogBag);

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

        var outDialogBag = dialogState.DialogBag.Add(sampleDialogRecord);

        dialogState = dialogState with
        {
            DialogBag = outDialogBag
        };

        Assert.Contains(dialogState.DialogBag, x => x == sampleDialogRecord);
    }
}